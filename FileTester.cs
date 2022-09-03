using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace echoStudy_webAPI.Tests
{
    [TestClass]
    public class FileTester
    {
        /**
         * Ensures the hash algorithm is one to one
         */
        [TestMethod]
        public void OneWayTest()
        {
            string x = getFileName("Hello World!", Language.English);
            string y = getFileName("Hello World!", Language.English);
            if(x != y)
            {
                Assert.Fail();
            }
        }

        /**
        * Tries to find collisions through using the hash naming scheme for 
        */
        [TestMethod]
        public void ValidityCollisionSeedTest()
        {
            Dictionary<string, Language> completedCards = new Dictionary<string, Language>();
            Dictionary<string, string> hashedCards = new Dictionary<string, string>();

            string japaneseSet = "ききます@listens|はなします@speaks|みます@looks (watches)|たべます@eats|のみます@drinks|たちます@stands|すわります@sits|あるきます@walks|はしります@runs|おきます@gets up|ねます@lies down|あげます@gives|もらいます@receives|おふろにはいります@takes a bath|かおをあらいます@washes face|てをあらいます@washes hands|あけます@opens|しめます@closes|きます@wears (torso)|ぬぎます@takes off (clothing)|はきます@wears (lower body)|おきます@leaves (places)|とります@takes|いきます@goes|きます@comes|さがします@searches|やすみます@rests|おちます@falls|すてます@throws away|ひろいます@picks up|おします@pushes|ひきます@pulls|のります@gets in (rides [a vehicle])|おります@gets off (gets out)|むすびます@ties|かみます@chews (bites)|ふきます@to blow (exhale)|さわります@touches|ゆびさします@points|いれます@puts in (inserts)|なげます@throws|けります@kicks|はをみがきます@brushes teeth|うがいをします@gargles|ならべます@lines up|えらびます@chooses|きめます@decides|おじぎをします@bows|あいします@loves|キスします@kisses|かんしゃします@thanks (is grateful for)|あやまります@apologizes|うたいます@sings|はくしゅします@claps (applauds)|おどります@dances|よみます@reads|かきます@writes|えがきます@draws|しっています@knows|かんがえます@ponders|わかります@understands|ききます@asks|かぞえます@counts|あそびます@plays|べんきょうします@studies|あいます@meets|きります@cuts|もやします@burns (sets fire to)|かいものをします@shops|たすけます@helps|ほめます@praises|しかります@scolds|はたらきます@works|よびます@calls (shouts for)|ほります@digs|うえます@plants|みずをやります@gives water to|こうかんします@exchanges|まちます@waits|ノックします@knocks|ゆうびんをだします@posts|るすばんします@stays at home|でんわをします@makes a phone call|いぬのさんぽをします@walks the dog|ちゅうしゃをします@gives a shot to|りょうりをします@cooks|そうじをします@cleans|トイレそうじをします@cleans the toilet|ふろそうじをします@cleans the bathtub|ふきます@wipes|ゴミをだします@puts the garbage out|ほします@puts out to dry|けっこんします@marries|けんかします@fights|でかけます@goes out|かえります@returns|うまれます@is born|しにます@dies|ふとっています@is overweight|やせています@is skinny|つかれます@is tired|やくそくします@to promise|いぬ@dog|ねこ@cat|うま@horse|うし@cow|ひつじ@sheep|コアラ@koala|カンガルー@kangaroo|ぞう@elephant|とら@tiger|ねずみ@mouse|うさぎ@rabbit|へび@snake|りんご@apple|レモン@lemon|いちご@strawberry|もも@peach|メロン@melon|バナナ@banana|さかな@fish|たまご@egg|オレンジ@orange|たまねぎ@onion|ぶどう@grapes|にんじん@carrot|トマト@tomato|ごはん@rice|ハンバーガー@hamburger|サンドイッチ@sandwich|すし@sushi|アイスクリーム@ice-cream|さんすう@maths|りか@science|しゃかい@social studies|おんがく@music|たいいく@P.E|としょ@library|しょどう@calligraphy|クラブ@after school club|コンピュータ@computer|えいご@English|にほんご@Japanese|まど@window|ドア@door|こくばん@blackboard|ほん@book|ノート@notebook|のり@glue|はさみ@scissors|けしゴム@rubber|えんぴつ@pencil|えんぴつけずり@pencil sharpener|ものさし/じょうぎ@ruler|いす@chair|つくえ@desk|とけい@clock|クリケット@cricket|すいえい@swimming|テニス@tennis|けんどう@kendo|じゅうどう@judo|からて@karate|じょうば@horse riding|ラグビー@rugby|スキー@skiing|サッカー@soccer/football|ネットボール@netball|バスケットボール@basketball|たいそう@gymnastics|すもう@sumo|バス@bus|でんしゃ@train|くるま@car|じてんしゃ@bicycle|しんかんせん@bullet train|ふね@ship|ひこうき@airplane|はれ@FINE weather|くもり@cloudy|あめ@rain|ゆき@snow|さむい@cold|あつい@hot|すずしい@cool|あたたかい@warm|でんわ@telephone|ぼうし@hat|くつ@shoes|かばん@bag|テレビ@television|こうちょうしつ@principal's office|しょくいんしつ@staff room|トイレ@toilet|ほけんしつ@sick bay|ばいてん@school kiosk|としょしつ@library|こたつ@Japanese table with heater|ざぶとん@Japanese cushion for tatami|ふとん@futon|おふろ@Japanese bath|たたみ@tatami|しょうじ@wood and rice paper screen|げんかん@entrance hall|うみ@sea|やま@mountain|こうえん@park|えいが@movie|ゆうえんち@amusement park|キャンプ@camp|けんだま@wooden toy|おりがみ@paper folding|たこ@kite|こま@spinning top|まんが@comics|スカート@skirt|ズボン@trousers|Tシャツ@t-shirts|セーター@jumper|ジャケット@jacket|きもの@kimono|め@eyes|みみ@ears|くち@mouth|はな@nose|あたま@head|かた@shoulder|ひざ@knee|あし@leg|おなか@stomach|むね@chest|かみのけ@hair|かお@face|て@hand|うで@arm|さむい@cold|あつい@hot|あたたかい@warm|すずしい@cool|あまい@sweet|からい@spicy|しょっぱい@salty|すっぱい@sour|にがい@bitter|たかい@Expensive/tall|やすい@cheap|みじかい@short|ながい@long|きれい（な）@Clean/pretty|きたない@dirty|おいしい@delicious|はやい@early/fast|おそい@late/slow|おもい@heavy|かるい@light|わかい@young|とし・としより@Old person|ちかい@near|おおきい@big|ちいさい@little|くらい@dark|あかるい@bright|とおい@far|ひろい@spacious|せまい@cramped|おおい@Many/much|すくない@few|つまらない@boring|おもしろい@Interesting/funny|あたらしい@new|ふるい@Old (object)|いい・よい@good|わるい@bad|いそがしい@busy|ひま （な）@Free (time)|げんき （な）@healthy|にぎやか （な）@lively|しずか （な）@quiet|じょうず （な）@skillful (for others)|とくい （な）@skillful (for self)|だいじ （な）@important|だめ （な）@No good|むり （な）@impossible|べんり （な）@convenient|あぶない@dangerous|あんぜん （な）@safe|すき （な）@like|しんせつ （な）@kind|しつれい （な）@rude|へた （な）@unskillful (for self)|にがて （な）@unskillful (for others)|きらい （な）@dislike|たのしい@enjoyable|あかい@red|あおい@Blue/green|みどり(いろ）（の）@green|くろい@black|しろい@white|きんぱつ@blond hair|ちゃいろい@brown|きいろい@yellow|むらさき (いろ）（の）@purple|いたい@hurt";
            if (!validCardSet(japaneseSet, Language.Japanese, Language.English, completedCards, hashedCards))
            {
                Assert.Fail();
            }

            string germanSet = "ausbleiben@to stay out|empfinden@to feel, experience|streiten@to argue|sich trennen@to separate|eine Familie grunden@to start a family|gemeinsame Interessen haben@to have interests in common|getrennt leben@to be separated|Interessen teilen@to share interests|kennenlernen@to get to know|gebraucht werden@to be needed|anklicken@to click on|ausprobieren@to try out, experiment with|chatten (im Internet, über Skype)@to chat (online, via Skype)|drehen@to film|empfehlen@to recommend|kommentieren@to comment|kommunizieren@to communicate|plazieren@to place (advertising)|posten@to post (online)|sichern@to secure|definieren@to define|die Möglichkeit bieten@to offer the possibility|hacken@to hack|hänseln@to tease, pick on|in Kontakt bleiben@to stay in contact|öffentlich machen@to make public|publizieren@to make public|schützen@to protect|taggen@to tag|veröffentlichen@to make public|versenden@to send|mit etwas aufwachsen@to grow up with sth|profitieren von@to profit from, take advantage of|recherchieren@to research|Angst um jemanden haben@to fear for/worry about somebody|anprobieren@to try on|eine Diät machen@to go on a diet|erlauben@to allow|sich schminken@to put on makeup|ein Selfie machen@to take a selfie|tranieren@to train (at the gym)|sich identifizieren mit@to identify with|Erfolg haben@to be successful|sich entspannen@to relax|produzieren@to produce|Musik auf einem Tablet hören@to listen to music on a tablet|Musik über YouTube hören@to listen to music on YouTube|beeinflussen@to influence|dumm machen@to make stupid|anerkennen@to recognise|stammen@to stem, originate|widmen@to dedicate, to devote|spenden@to donate|begeistern@to enthuse|Freundschaften schließen@to make friendships|melken@to milk|sich verkleiden@to dress up|vermitteln@to convey|sich begeistern@to enthuse|verbergen@to hide, conceal|sich (politisch) engagieren@to get involved (politically)|demonstrieren@to demonstrate|fliehen@to flee, escape|flüchten@to flee|gründen@to found|klettern auf@to climb on(to)|protestieren@to protest|stattfinden@to take place|der Berg, die Berge@mountain, mountains|das Bonbon, die Bonbons@sweet, candy|die Brücke, die Brücken@bridge, bridges|das Dorf, die Dörfer@village, villages|der Flughafen, die Flughäfen@airport, airports|der Fluss, die Flüsse@river, rivers|der Hafen, die Häfen@port, harbor|die Karte, die Karten@card; postcard; map|die Küste, die Küsten@coast, shore, seaside|der See, die Seen@lake, lakes|die Stadt, die Städte@city, town|der Turm, die Türme@tower, towers|der Ausgang, die Ausgänge@exit, exits|das Gepäck@luggage, baggage|der Rucksack, die Rucksäcke@backpack, backpacks|das Zelt, die Zelte@tent, tents|der Fotoapparat, die Fotoapparate@the camera, the cameras|das Geheimnis, die Geheimnisse@secret, secrets|der Hinweis, die Hinweise@hint, tip, clue|die Landschaft, die Landschaften@the landscape, the landscapes|die Limo@lemonade|der Schatz, die Schätze@treasure, treasures|das Schiff, die Schiffe@ship, ships|die Sonne@sun|der Vogel, die Vögel@bird, birds|der Anzug, die Anzüge@suit, suits|der Gürtel, die Gürtel@belt, belts|der Handschuh, die Handschuhe@glove/gloves|das Hemd, die Hemden@shirt, shirts|die Hose, die Hosen@trousers, pants|der Hut, die Hüte@hat, hats|das Kleid, die Kleider@dress, dresses|die Kleider@clothes|der Mantel, die Mäntel@coat, coats|der Pulli, die Pullis@sweater, pullover|die Sandale, die Sandalen@sandal, sandals|der Rock, die Röcke@skirt, skirts|der Schuh, die Schuhe@shoe/shoes|die Socke, die Socken@sock/socks|der Stiefel, die Stiefel@boot, boots|die Strumpfhose, die Strumpfhosen@tights, pantyhose|das Atelier, die Ateliers@studio, workshop|die Brille, die Brillen@glasses|der Nachbarn/die Nachbarin@neighbor|das Porträt, die Porträts@portrait|das Ziege@goat|das Zimmer, die Zimmer@room, rooms|die Ampel, die Ampeln@traffic light, traffic lights|der Fußgänger, die Fußgänger@pedestrian (male)|die Kreuzung, die Kreuzungen@crossing, intersection|die Höhle, die Höhlen@cave, caves|das Schloss, die Schlösser@castle, palace|das Tor, die Tore@gate|die Apotheke, die Apotheken@pharmacy, pharmacies|die Bäackerei@bakery|das Brot, die Brote@das Brot, die Brote|das Eis@ice cream|das Ei, die Eier@egg/eggs|die Erdbeere, die Erdbeeren@strawberry, strawberries|das Geschäft, die Geschäfte@shop|das Kilo, die Kilos@kilo, kilos|die Konditorei, die Konditoreien@pastry shop, pastry shops|der Korb, die Körbe@the basket, the baskets (sepet)|die Orange, die Orangen@orange, oranges|der Supermarkt, die Supermärkte@supermarket, supermarkets|der Brief, die Briefe@letter, letters|der Krebs, die Krebse@crab, cancer|der Witz, die Witze@the joke, the jokes|die Tür, die Türen@door/doors|das Foto, die Fotos@photo, photos|das Gemüse, die Gemüse@vegetable, vegetables|der Käse, die Käse@cheese, cheeses|die Prüfung, die Prüfungen@test; exam|die Schüssel, die Schüsseln@bowl, bowls|die Suppe, die Suppen@soup, soups|die Zeitung, die Zeitungen@newspaper, newspapers|die Frage, die Fragen, die Antwort, die Antworten@question, questions, answer, answers|der Garten, die Gärten@garden, gardens|das Gebäude, die Gebäude@the building, the buildings|der Hügel, die Hügel@hill/hills|die Kuh, die Kühe@cow, cows|der Kumpel, die Kumpels@buddy, mate|das Netz, die Netze@web, network, net|der Zettel, die Zettel@piece of paper|das Band, die Bänder@ribbon, strap, cord|die Idee, die Ideen@idea, ideas|das Zeichen, die Zeichen@sign, signs|das Teil, die Teile@part|das Tagebuch, die Tagebücher@diary, journal|die Taschenlampe; die Taschenlampen@torch, flashlight|die Aktentasche, die Aktentaschen@briefcase/briefcases|der Gauner, die Gauner@crook|der Lohn, die Löhne@wage, wages|der Papagei, die Papageien@parrot, parrots|die Pflanze, die Pflanzen@plant, plants|die Sammlung, die Sammlungen@collection, collections|das Verschwinden@disappearance|der Eingang, die Eingänge@the entrance, the entrances|das Mittagessen, die Mittagessen@the lunch, the lunches|die Sitzung, die Sitzungen@meeting(s), session(s)|der Stein, die Steine@stone, rock|der Kampf, die Kämpfe@fight, struggle|das Land, die Länder@country|der Zaun, die Zäune@the fence, the fences|das Denkmal, die Denkmäler@monument, monuments|das Laub@leaves, foliage(yeşillik)|die Nacht, die Nächte@at night, nights|die Richtung, die Richtungen@direction, directions|die Spur, die Spuren@trace, track, mark|das Vermögen, die Vermögen@assets, fortune, ability|das Versteck, die Verstecke@the hiding place, the hiding places|die Belohnung, die Belohnungen@reward/rewards|die Einzelheit, die Einzelheiten@detail, details|das Fenster, die Fenster@window, windows|das Handtuch, die Handtücher@hand towel|der Kerker, die Kerker@dungeon|die Mauer, die Mauern@wall, walls|der Verbrecher, die Verbrecher@criminal, criminals|der Anteil, die Anteile@portion, share|der Artikel, die Artikel@article, articles|der Dieb, die Diebe@thief, thieves|der Held, die Heldin@hero/heroine|der Kurier, die Kuriere@courier(kurye, haberci)|das Licht, die Lichter@light/lights|der Monat, die Monate@month/months|die Spalte, die Spalten@column, crack|die Treppe, die Treppen@staircase|die Weihnactsferien@christmas holidays|der Blitz, die Blitze@lightning/lightnings|der Frost, die Fröste@frost, frosts|das Gewitter, die Gewitter@thunderstorm, thunderstorms|der Hagel@hail(dolu)|der Himmel, die Himmel@sky, heaven|der Schnee@snow|die Wolke, die Wolken@cloud, clouds|groß@big, tall|Klein@small, short|faul@lazy|fleißig@hard-working|teuer@expensive|günstig@inexpensive|schön@beautiful|hässlich@ugly|glücklich@happy|traurig@sad|Hell@Light, bright|dunkle@dark|leicht@easy; light (in weight)|schwer@heavy (weight), difficult|begeistert@excited, enthusiastic|enttäuscht@disappointed|mutig@brave, courageous|Stark@Strong|dumm@stupid|schlau@smart|optimistisch@optimistic|pessimistisch@pessimistic|großzügig@generous|geizig@stingy|sauber@clean, neat|schmutzig@dirty|introvertiert@introverted|Extrovertiert@extroverted|schüchtern@shy, timid|aufgeschlossen@outgoing|schnell@fast|langsam@slow|gut@good, well|schlecht@bad, badly|Modern@Modern|altmodisch@old-fashioned|heiß@hot|kalt@cold|freundlich@friendly, kind|wichtig@important|Alt@Old|jung@young|neu@new|spät@late|früh@early|lebhaft@lively|kurz@short|lang@long|schmal@narrow|breit@wide|niedrig@low|hoch@high|Still@Quiet|anders@different|gleich@same|berühmt@famous, well-known|lustig@funny, amusing|kommisch@strange; weird|ruhig@calm|geschäftig@busy, bustling|laut@loud|frei@free|leise@quiet|einfach@easy|stressig@stressful|klug@smart, intelligent";
            if(!validCardSet(germanSet, Language.German, Language.English, completedCards, hashedCards))
            {
                Assert.Fail();
            }

            string spanishSet = "amar@to love|andar@to walk|ayudar@to help|bailar@to dance|buscar@to look for|cantar@to sing|cenar@to have diner|contestar@to answer|comprar@to buy|cocinar@to cook|desayunar@to have breakfest|entrar en@to enter|escuchar@to listen to|esperar@to wait, to hope|esquiar@to ski|estudiar@to study|hablar@to speak|lavar@to wash|limpiar@to clean|llegar@to arrive|llevar@to wear|mirar@to watch|montar@to ride|nadar@to swim|necesitar@to need|organizar@to organize|pagar@to pay|practicar@to practice|preguntar@to ask|preparar@to prepare|sacar@to take out|terminar@to finish|tocar@to touch, play an instrument|tomar@to take|trabajar@to work|visitar@to visit|aprender@to learn|beber@to drink|comer@to eat|cometer@to make a mistake|comprender@to understand|correr@to run|creer@to believe|deber@to owe, should|leer@to read|poseer@to possess|romper@to break|temer@to fear|vender@to sell|abrir@to open|admitir@to admit|asistir@to attend|cubrir@to cover|decidir@to decide|describir@to describe|descubrir@to discover|discutir@to discuss|escribir@to write|existir@to exsist|permitir@to permit|recibir@to recieve|subir@to climb|sufrir@to suffer|unir@to unite|vivir@to live|el supermercado@supermarket|la tienda@store|el día@day|el fin de semana@weekend|el domingo@Sunday|el jueves@Thursday|la película@movie|el número@number|el baloncesto@basketball|el béisbol@baseball|la cafetería@cafeteria|el chocolate@chocolate|la clase@class|el español@Spanish|el fútbol@soccer|el fútbol norteamericano@football|el jazz@jazz|la música@music|la pizza@pizza|la tarea@homework|el tenis@tennis|el voleibol@volleyball|el bolígrafo@pen|la calculadora@calculator|el lápiz@pencil|la librería@book store|el libro@book|el papel@paper|la mesa@table|el reloj@clock|la revista@magazine|la ropa@clothes|el dinero@money|hoy@today|mañana@tomorrow|las matemáticas@mathematics|la tarde@afternoon|el baile@dance|el concierto@concert|los deportes@sports|el examen@exam|el compañero@friend|el carro@car|el parque@park|la novela@novel|la teléfono@phone|el perro@dog|la guitarra@guitar|el helado@ice cream|la bicicleta@bicycle|el piano@piano|el restaurante@restaurant|el refresco@soft drink|la biblioteca@library|la casa@home|el centro@downtown|el cine@movie theater|la cocina@kitchen|la comida@food|el cuchillo@knife|cumpleaños@birthday|el desayuno@breakfast|el diciembre@December|el enero@January|la ensalada@salad|esposo@spouse|la estación@station or season|el febrero@February|la fiesta@party|la fruta@fruit|la hermana@sister|el hijo@son|el hombre@man|la hora@hour / time|inglés@English|el jefe@boss|los jueces@judges|julio@July|junio@June|la leche@milk|el limón@lemon|el lunes@Monday|la madre@mother|los maestros@teachers|la manzana@apple|el martes@Tuesday|marzo@March|mayo@May|el miércoles@Wednesday|un momento@moment|la mujer@woman|los mujeres@women|la naranja@orange|la niña@girl|el niño@boy|noche@night|novia@girlfriend|noviembre@November|novio@boyfriend|octubre@october|papá@dad|perdón@pardon|lo siento@sorry|la persona@person|el pescado@fish|la pimienta@pepper|el pollo@chicken|el sábado@Saturday|la sal@salt|septiembre@September|la sofá@sofa|la sopa@soup|el té@tea|las vacaciones@vacation|el vaso@drinking glass|viernes@Friday|el vino@wine|el huevo@egg|los pantalones@pants|la oficina@office|las camisas@shirts|la chaqueta@jacket|el/la jovén@young person|el boleto@ticket|el aeropuerto@airport|el pasaporte@passport|los dientes@teeth|la maleta@suitcase/bag|la farmacia@pharmacy|el enfermo@sick|el color@color|la vida@life|el amigo@friend|el idioma@language|la calle@street|el diente@tooth|el norte@north|el tenedor@fork|el sol@sun|la iglesia@church|el doctor@doctor|el joven@young person|la computadora@computer|el apartamento@apartment|el hotel@hotel|la camiseta@t-shirt|la blusa@blouse|el problema@problem|la ciudad@city|el estado@state|la vaca@cow|la falda@skirt|la familia@family|la fecha@date|la carta@letter|el gato@cat|los abuelos@grandparents|la semana@week|el hermano@brother|la hija@daughter|hijos@children|horas@hours|el cocinero@cook / chef|los colores@colors|los zapatos@shoes|el baño@bathroom|el vestido@dress|agosto@August|un trabajo@a job|el agua@water|los días@days|abril@April|la camisa@shirt|el profesor@professor / teacher|el sombrero@hat|¿Tienes tú@Do you have?|¿Necesitas tú@Do you need?|¿Quieres tu@Do you want?|¿Te gusta@Do you like?|Yo tengo@I have|Yo veo@I see|Yo necessito@I need|Yo quiero@I want|Me gusta@I like|la pared@wall|el chico@boy|la chica@girl|la tinta@ink|la amiga@female friend|el estudiante@student|la escuela@school|la pagina@page|el móvil@cellphone|el queso@cheese|la carne@meat|la hamburguesa@hamburger|el pan@bread|el tren@train|la estación de tren@train station|el autobús@bus|la musica@music|la fotografía@photograph|la foto@photo|la gasolina@gasoline|la medicina@medicine|el automóvil@automobile|el coche@car|un café@coffee shop|el teléfono celular@cell phone|el teatro@theater|la universidad@university|el banco@bank|la muchacha@girl|el padre@father";
            if (!validCardSet(spanishSet, Language.Spanish, Language.English, completedCards, hashedCards))
            {
                Assert.Fail();
            }

        }

        /**
         * Parses cardSet into multiple cards and will check if their file names are valid.
         * A file name is valid if it contains no illegal characters and doesn't contain a collision in the hash without being an exact duplicate
         */
        private bool validCardSet(string cardSet, Language frontLang, Language backLang,  Dictionary<string, Language> completedCards, Dictionary<string, string> hashedCards)
        {
            string[] cardSplit = cardSet.Split('|');
            foreach (string card in cardSplit)
            {
                // parse the card 
                string[] frontBack = card.Split('@');
                string front = frontBack[0];
                string back = frontBack[1];
                // hash the card terms
                string hashedFront = getFileName(front, frontLang);
                string hashedBack = getFileName(back, backLang);
                // check validity. it's valid if it doesn't already exist or it does already exist but is an exact duplicate.
                if (hashedCards.ContainsKey(hashedFront))
                {
                    if (!completedCards.ContainsKey(front))
                    {
                        return false;
                    }
                }
                if (hashedCards.ContainsKey(hashedBack))
                {
                    if (!completedCards.ContainsKey(back))
                    {
                        return false;
                    }
                }
                if (!IsValidFilename(hashedFront) || !IsValidFilename(hashedBack))
                {
                    return false;
                }
                // they're valid
                if (!completedCards.ContainsKey(front))
                {
                    completedCards.Add(front, Language.Spanish);
                }
                if (!completedCards.ContainsKey(back))
                {
                    completedCards.Add(back, Language.English);
                }
                if (!hashedCards.ContainsKey(hashedFront))
                {
                    hashedCards.Add(hashedFront, hashedFront);
                }
                if (!hashedCards.ContainsKey(hashedBack))
                {
                    hashedCards.Add(hashedBack, hashedBack);
                }
            }
            return true;
        }

        private bool IsValidFilename(string testName)
        {
            try
            {
                File.Create(testName).Dispose();
            }
            catch
            {
                return false;
            }

            // other checks for UNC, drive-path format, etc
            return true;
        }

        /**
         * Gets the file name that should be used for a given text and language
         */
        private static string getFileName(string text, Language language)
        {
            string fileName = language.ToString() + " " + text;

            SHA1 sha1 = SHA1.Create();

            byte[] hashedBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(fileName));

            fileName = ByteArrayToString(hashedBytes);

            return fileName + ".mp3";
        }

        /**
         * Taken from https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa
         * Converts a byte array to a a hexadecimal string
         */
        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba) 
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        private enum Language
        {
            English,
            Spanish,
            German,
            Japanese
        }
    }
}
