RESTClient, a debugger for RESTful web services.
https://addons.mozilla.org/en-US/firefox/addon/restclient/

####################################################

IISExpress has HTTP PUT & DELETE disabled by default

####################################################

Help Page of the BizMsgsService
http://localhost:3505/BizMsgService/

####################################################

HTTP GET - Gets all BizMsgs
http://localhost:3505/BizMsgService/

####################################################

HTTP GET - Gets a Single BizMsg
http://localhost:3505/BizMsgService/{id}

####################################################

HTTP POST - Creates a New BizMsg 
http://localhost:3505/BizMsgService/

User-Agent: Fiddler
Content-Type: application/xml

<BizMsg xmlns="http://schemas.datacontract.org/2004/07/NiCris.BusinessObjects">
<Date>2000-01-01</Date> 
<Name>String content</Name> 
<User>String content</User> 
</BizMsg>

<BizMsg xmlns="http://schemas.datacontract.org/2004/07/NiCris.BusinessObjects">
<AppId>String content</AppId> 
<Date>2000-01-01</Date>
<Description>String content</Description> 
<Name>String content</Name> 
<Roles>String content</Roles> 
<ServiceId>String content</ServiceId> 
<StyleId>String content</StyleId> 
<User>String content</User> 
</BizMsg>

####################################################

HTTP PUT - Updates an existing BizMsg
http://localhost:3505/BizMsgService/100

User-Agent: Fiddler
Content-Type: application/xml

<BizMsg xmlns="http://schemas.datacontract.org/2004/07/NiCris.BusinessObjects">
<Date>2000-01-01</Date>
<Name>String content</Name> 
<User>String content</User> 
</BizMsg>

<BizMsg xmlns="http://schemas.datacontract.org/2004/07/NiCris.BusinessObjects">
<AppId>String content</AppId> 
<Date>2000-01-01</Date>
<Description>String content</Description> 
<Name>String content</Name> 
<Roles>String content</Roles> 
<ServiceId>String content</ServiceId> 
<StyleId>String content</StyleId> 
<User>String content</User> 
</BizMsg>

####################################################

HTTP DELETE - Deletes an existing BizMsg
http://localhost:3505/BizMsgService/101