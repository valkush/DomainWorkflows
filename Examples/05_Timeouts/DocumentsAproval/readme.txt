# -----------------------------------------------------------------------------
ExpenseApproval workflow consists of reviewing and approving steps. After 
Expense document created workflow wait some time (15 sec – sliding timeout) 
to start reviewing. Reviewing Step has to be complete in reviving timeout time  
(45 sec) else, we get reviewing timeout and move to Overdue state. Approving 
Step has to be complete in 1 min else, we move to Overdue state. An expense
is considered as Approved when we pass both Reviewing and Approving steps 
on time.
# -----------------------------------------------------------------------------

### TESTING STEPS ###

# start new expense reviewing by document id
cmd > start 12

# update document several times in 15 sec window
cmd > update 12
cmd > update 12

# reviewing started in 15 sec since last update

# review the expense document
cmd > review 12

# approving is started

# decline the expense document
cmd > decline 12

#  workflow is complete with status DECLINED

# start new (or many) workflow with "start 22"
# HAVE FUN! )
