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


QUerying
==============

1. Search (post code or suburb name) - http://tnu.apphb.com/api/powerinterruption?suburb={postcode or 

suburb}
e.g http://tnu.apphb.com/api/powerinterruption?suburb=6102
    http://tnu.apphb.com/api/powerinterruption?suburb=Bentley

2. Search for suburbs with on going power interruptions
http://tnu.apphb.com/api/powerinterruption?suburb=servicedown
