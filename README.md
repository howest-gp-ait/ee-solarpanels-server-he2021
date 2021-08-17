# SOLAR PANELS SERVER SIDE

### CLONE DIT PROJECT NAAR JE LOKALE COMPUTER
### JE HOEFT AAN DEZE CODE NIETS TE WIJZIGEN
### JE START DE APPLICATIE EN JE START DE SERVER (noteer IP en poort).
  
#### JOUW CLIENT APP DIENT ALLEEN MAAR DE CORRECTE INSTRUCTIES NAAR DEZE APP TE STUREN EN INDIEN JE VRAGEN CORRECT ZIJN ZAL DE SERVER REAGEREN  
  

![demo](assets/server.png)  
  
**Achtergrond info over deze (server) app ... **

De server app maakt bij opstart een collectie van zonnepanelen aan :  
  * Het aantal panelen is random (van 3 tot 10 panelen)   
  * De grootte van de panelen is random (2, 3 of 4 m²)  
  * Het vermogen van een paneel wordt random bepaalt (tussen de 150 en 300W per m³)  
Verder valt er als gebruiker met deze app niets te doen.
Deze app zal via sockets reageren op verzoeken van eventuele clients (jouw werk dus).
De server begrijpt 3 instructies : 

    * **CONNECT##EOM**  
      Wanneer de server deze instructie ontvangt zal hij een string aanmaken die bestaat uit de ID's van de verschillende panelen, gescheiden door het | symbool.  
      Deze tekst wordt dan teruggestuurd naar de client.
      De server voegt GEEN extentie (##EOM) toe aan de tekst.
      De server toont de verstuurde tekst in zijn venster.
      Stel dat er 5 panelen werden aangemaakt dan zal volgende tekst teruggestuurd worden : **1|2|3|4|5**
      De client weet nu dat er 5 panelen zijn en dat het ID van het eerste paneel = 1, het ID van het tweede paneel = 2 enz.  De client (zie opdracht client app) zal met deze info een aantal controls in zijn scherm aanpassen.
      
    * **STATUS|1|2##EOM** 
      Het woord STATUS wordt dus gevolgd door een | symbool met daarna een getal dat verwijst naar het ID van een paneel (hier dus paneel met ID = 1).  Is dat getal gelijk aan 0 dan zal de status van alle panelen getoond worden.
      Na dit getal staat opnieuw een | symbool en opnieuw een getal dat de waarde 1, 2, 3 of 4 moet zijn.  Je zal straks zien in de client app dat dit geen probleem zal ijn gezien de waarden afkomstig zullen zijn van een (reeds gemaakte) slider.  Deze waarde heeft de huidige zonconditie aan.  
      Ter info  
        * 1 = zwaarbewolkt  
        * 2 = lichtbewolkt
        * 3 = overwegend zon
        * 4 = volle zon
      De tekst die de server zal retourneren (terug GEEN extensie ##EOM) zal de client dan in zijn venster afbeelden (zie opdracht client app).  
        
    * **ADD|3|250##EOM**  
      Het woord ADD wordt dus gevolgd door een | symbool met daarna een getal dat verwijst naar de oppervlakte van een nieuw paneel : dit **moeten** de waarden 2, 3, 4 of 5 zijn (hier dus 3). Je zal straks zien in de client app dat dit geen probleem zal zijn gezien de waarden afkomstig zullen zijn van een (reeds gemaakte) keuzelijst.  
      Dat getal wordt opnieuw gevolgd door een | symbool en vervolgens opnieuw een getal (hier dus 250).  Deze waarde staat voor het maximaal vermogen per m² en **moet* 150, 200, 250 of 300 zijn.  Je zal straks zien in de client app dat dit geen probleem zal zijn gezien de waarden afkomstig zullen zijn van een (reeds gemaakte) keuzelijst.  
      Wanneer de server deze opdracht ontvangt zal een extra paneel aangemaakt worden (het ID bepaalt de server zelf, de rest is op basis van de hier meegeleverde waarden).  
      Vervolgens zal de server hetzelfde terugsturen als bij de CONNECT instructie, namelijk een tekst met daarin alle ID's van de panelen, bijvoorbeeld **1|2|3|4|5|6** 
      De client zal op dat moment de inhoud van zijn controls terug bijstellen (zie opdracht client app).
      
      
      
      
      
      






