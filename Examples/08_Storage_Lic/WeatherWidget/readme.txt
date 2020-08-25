# -----------------------------------------------------------------------------
By default, workflow’s and scheduler state is stored in memory. It means the
state will be lost with next host restart. To save workflow and scheduler state 
to a durable storage like database, you need to buy a valid product license.
When you have a license you can uncomment UseSqlServerStorage() configuration
methods. If you use DomainWorkflows in commercial projects development, you need
to buy a license as well.
Weather workflow creates wheatear cache for requested region and keep it
actual during some time after the last request. All subsequent requests will be
extracted from the cached data. 
Demo application has five pre created regions with ids from 1 to 5.
# -----------------------------------------------------------------------------

### TESTING STEPS ###

# initially there is no cache data and no any active workflows

# request weather for region 1
# cmd > check 1

# region 1 cache created and start updating by timer

# [after some time] request weather for region 3
cmd > check 3
# region 3 cache created and start updating by timer

# [after some time] check how region 1 weather is changed
cmd > check 1

# HAVE FUN! )