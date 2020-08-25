# -----------------------------------------------------------------------------
Every workflow instance is addressed with a unique id – Workflow Key. Workflow key
is usually extracted from incoming event fields. By default, we use Id field
or combination of Id fields to extract a workflow key. If default extraction is not
satisfactory we can use [Key] attribute to change rules to extract a key from 
event fields.
Call Taxi workflow events is overloaded with different ids, so we use Key(“CallId”)
Attribute to use only one defined field for key extraction.
[LateKey] attribute tell us we don’t have key data when handling is started, but 
we will extract and provide the key data when the handler is running.
# -----------------------------------------------------------------------------

### TESTING STEPS ###

# call a taxi from addr1 to addr2
cmd > call addr1 addr2

# waiting call pickup or call timeout

#pickup the current call by id
cmd > pickup 1

# notify all parties and finish the call workflow

# we can start many call workflows simultaneously
# HAVE FUN! )

