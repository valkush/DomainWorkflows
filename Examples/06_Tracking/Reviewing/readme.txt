# -----------------------------------------------------------------------------
We can start article view process. System has a reviver’s chain defined by 
System policies. ReviewChain workflow ask every reviver to accept or reject 
the workflow article. An article is considered as approved when all reviewers 
Approved the article one by one.
We record every step of reviewing workflow with workflow tracking mechanism and 
then we can monitor all workflow steps. 
# -----------------------------------------------------------------------------

### TESTING STEPS ### 

# start new article review
cmd > start 1

# accept the article as reviewer 1
cmd > accept 1

# reject the article as reviewer 2
cmd > reject 2

# workflow is finished here with REJECTED status 

# check the workflow status and result
cmd > track 1

# track workflow steps
cmd trackx 1

# start new (or many) workflow with "start 2"
# HAVE FUN! )
