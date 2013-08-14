tnu-api
=======

A web api project acting as a service api for other projects/clients


*************************************************************
**************POWER INTERRUPTION API*************************
*************************************************************

Administration
==============
1. Update all interruption info from Western Power - http://tnu.apphb.com/Admin/LoadAllInterruptionInfo

2. Update all post code details from Australian Post - http://tnu.apphb.com/Admin/LoadPostCodeData

3. Get the last time data was updated - http://tnu.apphb.com/Admin/GetDataTimeStamp


Querying
==============

1. Search (post code or suburb name) - http://tnu.apphb.com/api/powerinterruption/{postcode or suburb}
e.g http://tnu.apphb.com/api/powerinterruption/6102
    http://tnu.apphb.com/api/powerinterruption/Bentley

2. Search for suburbs with on going power interruptions
http://tnu.apphb.com/api/powerinterruption/servicedown

3. Get the last modified timestamp
http://tnu.apphb.com/api/powerinterruption/GetLastUpdatedTimeStamp
