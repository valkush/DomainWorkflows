# -----------------------------------------------------------------------------
InvoiceWorkflow is used to ping payment status in a payment gateway. If the 
payment status is changed we update the related invoice. Every invoice has one
shadow invoice workflow, which starts when a new invoice is created. The 
workflow runs until its invoice payment is done or timeout expires.

InvoiceWorkflow workflow can be recovered when Payment Gateway is not available.
To indicate this we use [Recovery(typeof(PaymentServiceUnavailableException))] 
attribute. PaymentServiceUnavailableException is throwing randomly with a 
probability of 10 percent to emulate the service inaccessibility. Recovery 
process will retry the failed event and other queued events chain several times
to check the dependent service. 

All solution components are separated over several independent hosts. Communication
between the hosts is message based and use event channels. Each host configures
its event channel to send and receive events. Also hosts can import/export services
from/to other hosts. We use Connect and Publish methods for this purpose when 
configuring host’s endpoints. After Connect a service, it becomes available
transparently in host’s dependency injection container and can be used in any 
other components via DI. When we Publish a service, it starts listening and
handling any related channel events. To handle channel events Publish uses related
service from host’s DI container.

To test the application, we create a new invoice from CLIENT HOST. To create 
a new invoice CLIENT HOST calls application service IInvoiceService on APP SERVICES
HOST. After a new order created, IInvoiceService raises OrderCreated event, which 
starts InvoiceWorkflow on WORKFLOWS HOST. InvoiceWorkflow uses Payment Gateway to
create invoice’s payment and monitor its status using IPaymentGateway.

To connect hosts, we use RebusEventChannel adapter with RabbitMq. So, you need to
install the latest version of RabbitMq to test this example. We can configure and use
any other message brokers with RebusEventChannel like MSMQ, Asure Service Bus, SQL 
Server, MySql and so on.

We can start several host instances with the same endpoint name. In this case the 
instances will consume events concurrently, only one host will receive a message.
This allows us to scale workflow instances and manage system performance.
But If you have several hosts with the same endpoint name you MUST provide unique 
HostId for every host instance.
# -----------------------------------------------------------------------------

### TESTING STEPS ###
# type on CLIENT HOST

# check available commands
cmd > help

# create new invoice with total $10 and give 2 minutes to pay
cmd > create 10 2
# create one more
cmd > create 30 3 (total=30, 3 minutes to pay)

# update invoive[1] to total = $20
cmd > update 1  20

#check status of invoice[1]
cmd > check 1

#pay invoice[1] on Payment Gateway
cmd > pay 1 20
# check invoice[1] status again
cmd > check 1

# after 3 minutes check invoice[2] status
cmd > check 2

# start new (or many) workflow with "create total ttp"
# HAVE FUN! )
