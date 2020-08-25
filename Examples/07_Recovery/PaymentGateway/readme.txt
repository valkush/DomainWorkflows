# We have 10 pre-defined accounts for testing with next id and balance
# 1 - $10.0
# 2 - $20.0
# 3 - $30.0
# ...
# 10 - $100.0

# -----------------------------------------------------------------------------
We can pay form one account to another. Workflow try to authorize requested 
amount first. Then it will be auto-settle in 1 minute (Settle timeout) or
we can void the transaction without additional difficulties. If a transaction
has been settled we need to refund transaction.

When a workflow exception occurs, we can mark it as recoverable or 
nonrecoverable. If an exception is nonrecoverable the workflow finishes with 
Fault status. If an exception is recoverable, recovery process is started.
To mark an exception as recoverable, we need to override OnPreFault() method and 
return FaultAction.Recovery value. We can do the same with Recovery workflow 
attribute.

If Payment Service is temporarily unavailable, it throws ServiceUnavailableException
and we mark it as recoverable to start recovery process.
# -----------------------------------------------------------------------------

### TESTING STEPS ### 

# transfer $10 from acc 1 to acc 4
cmd > pay tx1 1 4 10

# disable PaymentService temporary
cmd > disable-ps

# SYSTEM cannot Settle the payment so start recovery process 

# restore PaymentService
cmd > enable-ps

# SYSTEM recovery the workflow and Settle the tx1 payment

# try to make next (tx2) payment form acc 1
cmd > pay tx2 1 7 10.0

# get non-recoverable Workflow Fault - Not enough money and complete the workflow with Fault result

# HAVE FUN! )
