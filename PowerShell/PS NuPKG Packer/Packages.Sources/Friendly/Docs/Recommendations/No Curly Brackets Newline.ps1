# Function example {
FrW_RunUnit { return 1 } { return 1 } { Write-Host "ERROR"} ([ref] ($private:errorOccured))
FrW_RunUnit 
	{ return 1 } { return 1 } { Write-Host "ERROR"} ([ref] ($private:errorOccured)) # errors because of spaced-parameters