#################### Interface for Debug Message Consumer Components*.
# * Note: These Components are hard-coded in the Distributor for perfomance reasons. 
#
# Debug Message (DM) Consumer (DMCo) Components must have:
# + *GetLevel		- Gets the Component's Trehshold Level.
# + *SetLevel		- Sets the Component's Threshold Level (returns 0/1).
# + *GetSubscription - Gets the Component's Subscribed to Creator.
# + *SetSubscription - Sets the Component's Subscribed to Creator (returns 0/1).
# + *Supply			- Used by the DM-Distributor to supply a DM to the DMCo.
#                     Note: The DM-Distributor only Supplies DMs whose Level & Subscription are in conformance.
#
# - No calls to Trace, since this creates a loop.