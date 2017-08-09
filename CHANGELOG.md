## 2017-08-07 v1.1.1

**Autor: Iker Landajuela <ikernaix@gmail.com>**

* `SendResponse`: Logeamos propiedad llamada cliente HTTP. Con `request.HttpMethod` obtenemos si es GET o POST.	
* `Run`: Pruebas con context Request.QueryString para obtener parametros query URL GET
* `Run`: Código HTTP respuesta con `ctx.Response.StatusCode = (int)HttpStatusCode.OK;`


## 2017-08-07 v1.1.0

**Autor: Iker Landajuela <ikernaix@gmail.com>**

* Nueva clase WebServer con ejemplo servidor HTTP sencillo basada en HttpListener


## 2017-08-05 v1.0.1

**Autor: Iker Landajuela <ikernaix@gmail.com>**

* `eventLog1`: Era necesario definir la propiedad Source ("MyNewService") para que se muestre bien en visor de eventos de Win.

* Scripts Powershell para instalar, ver, parar...servicio en carpeta Debug

* Elimino instalador servicio ProjectInstaller y instalo a mano con scripts PS.

* variable `AppLogEvtId` con tipos de evento de la aplicación para pasar a `EventLog.WriteEntry` como ID de evento.

* `eventLog1`: Propiedad AutoLog a False para que no genere eventos extra.

* `Service1.cs` Pruebas de escritura de evento de error.

* Instalo NLog

`Install-Package NLog`

`Install-Package NLog.Config`



