using MassTransit;

namespace Windetta.Operations.Messages.Events;

[EntityName("identity.UserCreated")]
[MessageUrn("Windetta.Identity.Messages.Events:UserCreated")]
public class UserCreated
{
    public Guid Id { get; set; }
}

//{

//  "messageId": "c46e0000-c148-d8bb-3fad-08dba235a7f2",

//  "requestId": null,

//  "correlationId": null,

//  "conversationId": "c46e0000-c148-d8bb-57a1-08dba235a7f4",

//  "initiatorId": null,

//  "sourceAddress": "rabbitmq://localhost/DESKTOP933U206_WindettaIdentity_bus_atzyyygbjdcmzcswbdp4rpm9d5?temporary=true",

//  "destinationAddress": "rabbitmq://localhost/identity.UserCreated",

//  "responseAddress": null,

//  "faultAddress": null,

//  "messageType": [

//    "urn:message:Windetta.Operations.Messages.Events:UserCreated"

//  ],

//  "message": {

//    "id": "d30a42ba-d4af-4d47-b00d-acffaf974a54",

//    "userName": "user@vk.450477125",

//    "email": "neuralink7232050@gmail.com",

//    "role": "user"

//  },

//  "expirationTime": null,

//  "sentTime": "2023-08-21T10:59:13.7694637Z",

//  "headers": { },

//  "host": {

//    "machineName": "DESKTOP-933U206",

//    "processName": "Windetta.Identity",

//    "processId": 28356,

//    "assembly": "Windetta.Identity",

//    "assemblyVersion": "1.0.0.0",

//    "frameworkVersion": "7.0.5",

//    "massTransitVersion": "8.1.0.0",

//    "operatingSystemVersion": "Microsoft Windows NT 10.0.22621.0"

//  }

//}