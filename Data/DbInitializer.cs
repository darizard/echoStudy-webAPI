using echoStudy_webAPI.Areas.Identity.Data;
using echoStudy_webAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace echoStudy_webAPI.Data
{
    public class DbInitializer
    {
        /**
         * Initializes EchoStudyDB and seeds data
         */
        public static async Task InitializeEchoStudyDb(EchoStudyDB echoContext, UserManager<EchoUser> userManager)
        {
            // Seed data if none exists
            if (!echoContext.Decks.Any() && !echoContext.DeckCategories.Any() && !echoContext.Cards.Any())
            {
                await addLanguageMaterial(echoContext, userManager);
            }
        }

        public static void CreateEchoStudyDB(EchoStudyDB echoContext, EchoStudyDB identityContext)
        {
            // Create the DB if it isn't already created
            if(echoContext.Database.EnsureCreated() == false)
            {
                // Migrations
                echoContext.Database.Migrate();
                identityContext.Database.Migrate();
            }
        }

        /**
         * Adds decks, deckcategories, and cards related to language to an EchoStudyDB 
         */
        public static async Task addLanguageMaterial(EchoStudyDB context, UserManager<EchoUser> userManager)
        {
            EchoUser user1 = await userManager.FindByEmailAsync("JohnDoe@gmail.com");
            addGermanMaterial(context, user1);
            EchoUser user2 = await userManager.FindByEmailAsync("JaneDoe@gmail.com");
            addJapaneseMaterial(context, user2);
            EchoUser user3 = await userManager.FindByEmailAsync("JohnSmith@gmail.com");
            addSpanishMaterial(context, user3);
        }

        /**
        * Adds three decks to an EchoStudyDB that contain Japanese/English cards as well as a Japanese category.
        * The given EchoUser will be marked as the owner of everything initialized
        * Two of them are public, one of them are private
        */
        private static void addJapaneseMaterial(EchoStudyDB context, EchoUser owner)
        {
            // Category for Japanese
            DeckCategory langCategory = new DeckCategory();
            langCategory.Decks = new List<Deck>();
            langCategory.Title = "Japanese";
            langCategory.Description = "Decks that are useful for learning Japanese";
            langCategory.UserId = owner.Id;
            langCategory.DateCreated = DateTime.Now;
            langCategory.DateTouched = DateTime.Now;
            langCategory.DateUpdated = DateTime.Now;

            // First Japanese Deck
            Deck japaneseDeck1 = new Deck();
            japaneseDeck1.DeckCategories = new List<DeckCategory>();
            japaneseDeck1.Cards = new List<Card>();
            japaneseDeck1.Title = "Easy Japanese Verbs";
            japaneseDeck1.Description = "Introductory Japanese verbs helpful for people trying to learn Japanese. Verbs are in polite form (～ます)";
            japaneseDeck1.Access = Access.Public;
            japaneseDeck1.DefaultFrontLang = Language.Japanese;
            japaneseDeck1.DefaultBackLang = Language.English;
            japaneseDeck1.UserId = owner.Id;
            DateTime date1 = DateTime.Now.Add(randomTimeSpan());
            japaneseDeck1.DateCreated = date1;
            japaneseDeck1.DateTouched = date1.Add(randomTimeSpan());
            japaneseDeck1.DateUpdated = date1.Add(randomTimeSpan());
            // First Japanese Deck's flash cards
            Card[] japaneseCards1 = createFlashCards(japaneseDeck1, "ききます@listens|はなします@speaks|みます@looks (watches)|たべます@eats|のみます@drinks|たちます@stands|すわります@sits|あるきます@walks|はしります@runs|おきます@gets up|ねます@lies down|あげます@gives|もらいます@receives|おふろにはいります@takes a bath|かおをあらいます@washes face|てをあらいます@washes hands|あけます@opens|しめます@closes|きます@wears (torso)|ぬぎます@takes off (clothing)|はきます@wears (lower body)|おきます@leaves (places)|とります@takes|いきます@goes|きます@comes|さがします@searches|やすみます@rests|おちます@falls|すてます@throws away|ひろいます@picks up|おします@pushes|ひきます@pulls|のります@gets in (rides [a vehicle])|おります@gets off (gets out)|むすびます@ties|かみます@chews (bites)|ふきます@to blow (exhale)|さわります@touches|ゆびさします@points|いれます@puts in (inserts)|なげます@throws|けります@kicks|はをみがきます@brushes teeth|うがいをします@gargles|ならべます@lines up|えらびます@chooses|きめます@decides|おじぎをします@bows|あいします@loves|キスします@kisses|かんしゃします@thanks (is grateful for)|あやまります@apologizes|うたいます@sings|はくしゅします@claps (applauds)|おどります@dances|よみます@reads|かきます@writes|えがきます@draws|しっています@knows|かんがえます@ponders|わかります@understands|ききます@asks|かぞえます@counts|あそびます@plays|べんきょうします@studies|あいます@meets|きります@cuts|もやします@burns (sets fire to)|かいものをします@shops|たすけます@helps|ほめます@praises|しかります@scolds|はたらきます@works|よびます@calls (shouts for)|ほります@digs|うえます@plants|みずをやります@gives water to|こうかんします@exchanges|まちます@waits|ノックします@knocks|ゆうびんをだします@posts|るすばんします@stays at home|でんわをします@makes a phone call|いぬのさんぽをします@walks the dog|ちゅうしゃをします@gives a shot to|りょうりをします@cooks|そうじをします@cleans|トイレそうじをします@cleans the toilet|ふろそうじをします@cleans the bathtub|ふきます@wipes|ゴミをだします@puts the garbage out|ほします@puts out to dry|けっこんします@marries|けんかします@fights|でかけます@goes out|かえります@returns|うまれます@is born|しにます@dies|ふとっています@is overweight|やせています@is skinny|つかれます@is tired|やくそくします@to promise",
                Language.Japanese, Language.English, owner);
            // Associate the deck and category
            langCategory.Decks.Add(japaneseDeck1);
            japaneseDeck1.DeckCategories.Add(langCategory);

            // Second Japanese Deck
            Deck japaneseDeck2 = new Deck();
            japaneseDeck2.DeckCategories = new List<DeckCategory>();
            japaneseDeck2.Cards = new List<Card>();
            japaneseDeck2.Title = "Easy Japanese Nouns";
            japaneseDeck2.Description = "Introductory Japanese nouns helpful for people trying to learn Japanese";
            japaneseDeck2.Access = Access.Public;
            japaneseDeck2.DefaultFrontLang = Language.Japanese;
            japaneseDeck2.DefaultBackLang = Language.English;
            japaneseDeck2.UserId = owner.Id;
            DateTime date2 = DateTime.Now.Add(randomTimeSpan());
            japaneseDeck2.DateCreated = date2;
            japaneseDeck2.DateTouched = date2.Add(randomTimeSpan());
            japaneseDeck2.DateUpdated = date2.Add(randomTimeSpan());
            // Second Japanese Deck's flash cards
            Card[] japaneseCards2 = createFlashCards(japaneseDeck2, "いぬ@dog|ねこ@cat|うま@horse|うし@cow|ひつじ@sheep|コアラ@koala|カンガルー@kangaroo|ぞう@elephant|とら@tiger|ねずみ@mouse|うさぎ@rabbit|へび@snake|りんご@apple|レモン@lemon|いちご@strawberry|もも@peach|メロン@melon|バナナ@banana|さかな@fish|たまご@egg|オレンジ@orange|たまねぎ@onion|ぶどう@grapes|にんじん@carrot|トマト@tomato|ごはん@rice|ハンバーガー@hamburger|サンドイッチ@sandwich|すし@sushi|アイスクリーム@ice-cream|さんすう@maths|りか@science|しゃかい@social studies|おんがく@music|たいいく@P.E|としょ@library|しょどう@calligraphy|クラブ@after school club|コンピュータ@computer|えいご@English|にほんご@Japanese|まど@window|ドア@door|こくばん@blackboard|ほん@book|ノート@notebook|のり@glue|はさみ@scissors|けしゴム@rubber|えんぴつ@pencil|えんぴつけずり@pencil sharpener|ものさし/じょうぎ@ruler|いす@chair|つくえ@desk|とけい@clock|クリケット@cricket|すいえい@swimming|テニス@tennis|けんどう@kendo|じゅうどう@judo|からて@karate|じょうば@horse riding|ラグビー@rugby|スキー@skiing|サッカー@soccer/football|ネットボール@netball|バスケットボール@basketball|たいそう@gymnastics|すもう@sumo|バス@bus|でんしゃ@train|くるま@car|じてんしゃ@bicycle|しんかんせん@bullet train|ふね@ship|ひこうき@airplane|はれ@FINE weather|くもり@cloudy|あめ@rain|ゆき@snow|さむい@cold|あつい@hot|すずしい@cool|あたたかい@warm|でんわ@telephone|ぼうし@hat|くつ@shoes|かばん@bag|テレビ@television|こうちょうしつ@principal's office|しょくいんしつ@staff room|トイレ@toilet|ほけんしつ@sick bay|ばいてん@school kiosk|としょしつ@library|こたつ@Japanese table with heater|ざぶとん@Japanese cushion for tatami|ふとん@futon|おふろ@Japanese bath|たたみ@tatami|しょうじ@wood and rice paper screen|げんかん@entrance hall|うみ@sea|やま@mountain|こうえん@park|えいが@movie|ゆうえんち@amusement park|キャンプ@camp|けんだま@wooden toy|おりがみ@paper folding|たこ@kite|こま@spinning top|まんが@comics|スカート@skirt|ズボン@trousers|Tシャツ@t-shirts|セーター@jumper|ジャケット@jacket|きもの@kimono|め@eyes|みみ@ears|くち@mouth|はな@nose|あたま@head|かた@shoulder|ひざ@knee|あし@leg|おなか@stomach|むね@chest|かみのけ@hair|かお@face|て@hand|うで@arm",
                Language.Japanese, Language.English, owner);
            // Associate the deck and category
            langCategory.Decks.Add(japaneseDeck2);
            japaneseDeck2.DeckCategories.Add(langCategory);

            // Third Japanese Deck
            Deck japaneseDeck3 = new Deck();
            japaneseDeck3.DeckCategories = new List<DeckCategory>();
            japaneseDeck3.Cards = new List<Card>();
            japaneseDeck3.Title = "Easy Japanese Adjectives";
            japaneseDeck3.Description = "Introductory Japanese adjectives helpful for people trying to learn Japanese";
            japaneseDeck3.Access = Access.Private;
            japaneseDeck3.DefaultFrontLang = Language.Japanese;
            japaneseDeck3.DefaultBackLang = Language.English;
            japaneseDeck3.UserId = owner.Id;
            DateTime date3 = DateTime.Now.Add(randomTimeSpan());
            japaneseDeck3.DateCreated = date3;
            japaneseDeck3.DateTouched = date3.Add(randomTimeSpan());
            japaneseDeck3.DateUpdated = date3.Add(randomTimeSpan());
            // Third Japanese Deck's flash cards
            Card[] japaneseCards3 = createFlashCards(japaneseDeck3, "さむい@cold|あつい@hot|あたたかい@warm|すずしい@cool|あまい@sweet|からい@spicy|しょっぱい@salty|すっぱい@sour|にがい@bitter|たかい@Expensive/tall|やすい@cheap|みじかい@short|ながい@long|きれい（な）@Clean/pretty|きたない@dirty|おいしい@delicious|はやい@early/fast|おそい@late/slow|おもい@heavy|かるい@light|わかい@young|とし・としより@Old person|ちかい@near|おおきい@big|ちいさい@little|くらい@dark|あかるい@bright|とおい@far|ひろい@spacious|せまい@cramped|おおい@Many/much|すくない@few|つまらない@boring|おもしろい@Interesting/funny|あたらしい@new|ふるい@Old (object)|いい・よい@good|わるい@bad|いそがしい@busy|ひま （な）@Free (time)|げんき （な）@healthy|にぎやか （な）@lively|しずか （な）@quiet|じょうず （な）@skillful (for others)|とくい （な）@skillful (for self)|だいじ （な）@important|だめ （な）@No good|むり （な）@impossible|べんり （な）@convenient|あぶない@dangerous|あんぜん （な）@safe|すき （な）@like|しんせつ （な）@kind|しつれい （な）@rude|へた （な）@unskillful (for self)|にがて （な）@unskillful (for others)|きらい （な）@dislike|たのしい@enjoyable|あかい@red|あおい@Blue/green|みどり(いろ）（の）@green|くろい@black|しろい@white|きんぱつ@blond hair|ちゃいろい@brown|きいろい@yellow|むらさき (いろ）（の）@purple|いたい@hurt",
                Language.Japanese, Language.English, owner);
            // Associate the deck and category
            langCategory.Decks.Add(japaneseDeck3);
            japaneseDeck3.DeckCategories.Add(langCategory);

            // Add everything to the database and save
            context.DeckCategories.Add(langCategory);

            PositionInitializer.Init();
            context.Decks.Add(japaneseDeck1);
            foreach (Card card in japaneseCards1)
            {
                card.DeckPosition = PositionInitializer.Next();
                context.Cards.Add(card);
            }

            PositionInitializer.Init();
            context.Decks.Add(japaneseDeck2);
            foreach (Card card in japaneseCards2)
            {
                card.DeckPosition = PositionInitializer.Next();
                context.Cards.Add(card);
            }

            PositionInitializer.Init();
            context.Decks.Add(japaneseDeck3);
            foreach (Card card in japaneseCards3)
            {
                card.DeckPosition = PositionInitializer.Next();
                context.Cards.Add(card);
            }

            context.SaveChanges();
        }

        /**
         * Adds three decks to an EchoStudyDB that contain German/English cards as well as a German category.
         * The given EchoUser will be marked as the owner of everything initialized
         * Two of them are public, one of them are private
         */
        private static void addGermanMaterial(EchoStudyDB context, EchoUser owner)
        {
            // Category for German
            DeckCategory langCategory = new DeckCategory();
            langCategory.Decks = new List<Deck>();
            langCategory.Title = "German";
            langCategory.Description = "Decks that are useful for learning German";
            langCategory.UserId = owner.Id;
            langCategory.DateCreated = DateTime.Now;
            langCategory.DateTouched = DateTime.Now;
            langCategory.DateUpdated = DateTime.Now;

            // First German Deck
            Deck germanDeck1 = new Deck();
            germanDeck1.DeckCategories = new List<DeckCategory>();
            germanDeck1.Cards = new List<Card>();
            germanDeck1.Title = "Easy German Verbs";
            germanDeck1.Description = "Introductory German verbs helpful for people trying to learn German";
            germanDeck1.Access = Access.Public;
            germanDeck1.DefaultFrontLang = Language.German;
            germanDeck1.DefaultBackLang = Language.English;
            germanDeck1.UserId = owner.Id;
            DateTime date1 = DateTime.Now.Add(randomTimeSpan());
            germanDeck1.DateCreated = date1;
            germanDeck1.DateTouched = date1.Add(randomTimeSpan());
            germanDeck1.DateUpdated = date1.Add(randomTimeSpan());
            // First German Deck's flash cards
            Card[] germanCards1 = createFlashCards(germanDeck1, "ausbleiben@to stay out|empfinden@to feel, experience|streiten@to argue|sich trennen@to separate|eine Familie grunden@to start a family|gemeinsame Interessen haben@to have interests in common|getrennt leben@to be separated|Interessen teilen@to share interests|kennenlernen@to get to know|gebraucht werden@to be needed|anklicken@to click on|ausprobieren@to try out, experiment with|chatten (im Internet, über Skype)@to chat (online, via Skype)|drehen@to film|empfehlen@to recommend|kommentieren@to comment|kommunizieren@to communicate|plazieren@to place (advertising)|posten@to post (online)|sichern@to secure|definieren@to define|die Möglichkeit bieten@to offer the possibility|hacken@to hack|hänseln@to tease, pick on|in Kontakt bleiben@to stay in contact|öffentlich machen@to make public|publizieren@to make public|schützen@to protect|taggen@to tag|veröffentlichen@to make public|versenden@to send|mit etwas aufwachsen@to grow up with sth|profitieren von@to profit from, take advantage of|recherchieren@to research|Angst um jemanden haben@to fear for/worry about somebody|anprobieren@to try on|eine Diät machen@to go on a diet|erlauben@to allow|sich schminken@to put on makeup|ein Selfie machen@to take a selfie|tranieren@to train (at the gym)|sich identifizieren mit@to identify with|Erfolg haben@to be successful|sich entspannen@to relax|produzieren@to produce|Musik auf einem Tablet hören@to listen to music on a tablet|Musik über YouTube hören@to listen to music on YouTube|beeinflussen@to influence|dumm machen@to make stupid|anerkennen@to recognise|stammen@to stem, originate|widmen@to dedicate, to devote|spenden@to donate|begeistern@to enthuse|Freundschaften schließen@to make friendships|melken@to milk|sich verkleiden@to dress up|vermitteln@to convey|sich begeistern@to enthuse|verbergen@to hide, conceal|sich (politisch) engagieren@to get involved (politically)|demonstrieren@to demonstrate|fliehen@to flee, escape|flüchten@to flee|gründen@to found|klettern auf@to climb on(to)|protestieren@to protest|stattfinden@to take place",
                Language.German, Language.English, owner);
            // Associate the deck and category
            langCategory.Decks.Add(germanDeck1);
            germanDeck1.DeckCategories.Add(langCategory);

            // Second German Deck
            Deck germanDeck2 = new Deck();
            germanDeck2.DeckCategories = new List<DeckCategory>();
            germanDeck2.Cards = new List<Card>();
            germanDeck2.Title = "Easy German Nouns";
            germanDeck2.Description = "Introductory German nouns helpful for people trying to learn German";
            germanDeck2.Access = Access.Public;
            germanDeck2.DefaultFrontLang = Language.German;
            germanDeck2.DefaultBackLang = Language.English;
            germanDeck2.UserId = owner.Id;
            DateTime date2 = DateTime.Now.Add(randomTimeSpan());
            germanDeck2.DateCreated = date2;
            germanDeck2.DateTouched = date2.Add(randomTimeSpan());
            germanDeck2.DateUpdated = date2.Add(randomTimeSpan());
            // Second German Deck's flash cards
            Card[] germanCards2 = createFlashCards(germanDeck2, "der Berg, die Berge@mountain, mountains|das Bonbon, die Bonbons@sweet, candy|die Brücke, die Brücken@bridge, bridges|das Dorf, die Dörfer@village, villages|der Flughafen, die Flughäfen@airport, airports|der Fluss, die Flüsse@river, rivers|der Hafen, die Häfen@port, harbor|die Karte, die Karten@card; postcard; map|die Küste, die Küsten@coast, shore, seaside|der See, die Seen@lake, lakes|die Stadt, die Städte@city, town|der Turm, die Türme@tower, towers|der Ausgang, die Ausgänge@exit, exits|das Gepäck@luggage, baggage|der Rucksack, die Rucksäcke@backpack, backpacks|das Zelt, die Zelte@tent, tents|der Fotoapparat, die Fotoapparate@the camera, the cameras|das Geheimnis, die Geheimnisse@secret, secrets|der Hinweis, die Hinweise@hint, tip, clue|die Landschaft, die Landschaften@the landscape, the landscapes|die Limo@lemonade|der Schatz, die Schätze@treasure, treasures|das Schiff, die Schiffe@ship, ships|die Sonne@sun|der Vogel, die Vögel@bird, birds|der Anzug, die Anzüge@suit, suits|der Gürtel, die Gürtel@belt, belts|der Handschuh, die Handschuhe@glove/gloves|das Hemd, die Hemden@shirt, shirts|die Hose, die Hosen@trousers, pants|der Hut, die Hüte@hat, hats|das Kleid, die Kleider@dress, dresses|die Kleider@clothes|der Mantel, die Mäntel@coat, coats|der Pulli, die Pullis@sweater, pullover|die Sandale, die Sandalen@sandal, sandals|der Rock, die Röcke@skirt, skirts|der Schuh, die Schuhe@shoe/shoes|die Socke, die Socken@sock/socks|der Stiefel, die Stiefel@boot, boots|die Strumpfhose, die Strumpfhosen@tights, pantyhose|das Atelier, die Ateliers@studio, workshop|die Brille, die Brillen@glasses|der Nachbarn/die Nachbarin@neighbor|das Porträt, die Porträts@portrait|das Ziege@goat|das Zimmer, die Zimmer@room, rooms|die Ampel, die Ampeln@traffic light, traffic lights|der Fußgänger, die Fußgänger@pedestrian (male)|die Kreuzung, die Kreuzungen@crossing, intersection|die Höhle, die Höhlen@cave, caves|das Schloss, die Schlösser@castle, palace|das Tor, die Tore@gate|die Apotheke, die Apotheken@pharmacy, pharmacies|die Bäackerei@bakery|das Brot, die Brote@das Brot, die Brote|das Eis@ice cream|das Ei, die Eier@egg/eggs|die Erdbeere, die Erdbeeren@strawberry, strawberries|das Geschäft, die Geschäfte@shop|das Kilo, die Kilos@kilo, kilos|die Konditorei, die Konditoreien@pastry shop, pastry shops|der Korb, die Körbe@the basket, the baskets (sepet)|die Orange, die Orangen@orange, oranges|der Supermarkt, die Supermärkte@supermarket, supermarkets|der Brief, die Briefe@letter, letters|der Krebs, die Krebse@crab, cancer|der Witz, die Witze@the joke, the jokes|die Tür, die Türen@door/doors|das Foto, die Fotos@photo, photos|das Gemüse, die Gemüse@vegetable, vegetables|der Käse, die Käse@cheese, cheeses|die Prüfung, die Prüfungen@test; exam|die Schüssel, die Schüsseln@bowl, bowls|die Suppe, die Suppen@soup, soups|die Zeitung, die Zeitungen@newspaper, newspapers|die Frage, die Fragen, die Antwort, die Antworten@question, questions, answer, answers|der Garten, die Gärten@garden, gardens|das Gebäude, die Gebäude@the building, the buildings|der Hügel, die Hügel@hill/hills|die Kuh, die Kühe@cow, cows|der Kumpel, die Kumpels@buddy, mate|das Netz, die Netze@web, network, net|der Zettel, die Zettel@piece of paper|das Band, die Bänder@ribbon, strap, cord|die Idee, die Ideen@idea, ideas|das Zeichen, die Zeichen@sign, signs|das Teil, die Teile@part|das Tagebuch, die Tagebücher@diary, journal|die Taschenlampe; die Taschenlampen@torch, flashlight|die Aktentasche, die Aktentaschen@briefcase/briefcases|der Gauner, die Gauner@crook|der Lohn, die Löhne@wage, wages|der Papagei, die Papageien@parrot, parrots|die Pflanze, die Pflanzen@plant, plants|die Sammlung, die Sammlungen@collection, collections|das Verschwinden@disappearance|der Eingang, die Eingänge@the entrance, the entrances|das Mittagessen, die Mittagessen@the lunch, the lunches|die Sitzung, die Sitzungen@meeting(s), session(s)|der Stein, die Steine@stone, rock|der Kampf, die Kämpfe@fight, struggle|das Land, die Länder@country|der Zaun, die Zäune@the fence, the fences|das Denkmal, die Denkmäler@monument, monuments|das Laub@leaves, foliage(yeşillik)|die Nacht, die Nächte@at night, nights|die Richtung, die Richtungen@direction, directions|die Spur, die Spuren@trace, track, mark|das Vermögen, die Vermögen@assets, fortune, ability|das Versteck, die Verstecke@the hiding place, the hiding places|die Belohnung, die Belohnungen@reward/rewards|die Einzelheit, die Einzelheiten@detail, details|das Fenster, die Fenster@window, windows|das Handtuch, die Handtücher@hand towel|der Kerker, die Kerker@dungeon|die Mauer, die Mauern@wall, walls|der Verbrecher, die Verbrecher@criminal, criminals|der Anteil, die Anteile@portion, share|der Artikel, die Artikel@article, articles|der Dieb, die Diebe@thief, thieves|der Held, die Heldin@hero/heroine|der Kurier, die Kuriere@courier(kurye, haberci)|das Licht, die Lichter@light/lights|der Monat, die Monate@month/months|die Spalte, die Spalten@column, crack|die Treppe, die Treppen@staircase|die Weihnactsferien@christmas holidays|der Blitz, die Blitze@lightning/lightnings|der Frost, die Fröste@frost, frosts|das Gewitter, die Gewitter@thunderstorm, thunderstorms|der Hagel@hail(dolu)|der Himmel, die Himmel@sky, heaven|der Schnee@snow|die Wolke, die Wolken@cloud, clouds",
                Language.German, Language.English, owner);
            // Associate the deck and category
            langCategory.Decks.Add(germanDeck2);
            germanDeck2.DeckCategories.Add(langCategory);

            // Third German Deck
            Deck germanDeck3 = new Deck();
            germanDeck3.DeckCategories = new List<DeckCategory>();
            germanDeck3.Cards = new List<Card>();
            germanDeck3.Title = "Easy German Adjectives";
            germanDeck3.Description = "Introductory German adjectives helpful for people trying to learn German";
            germanDeck3.Access = Access.Private;
            germanDeck3.DefaultFrontLang = Language.German;
            germanDeck3.DefaultBackLang = Language.English;
            germanDeck3.UserId = owner.Id;
            DateTime date3 = DateTime.Now.Add(randomTimeSpan());
            germanDeck3.DateCreated = date3;
            germanDeck3.DateTouched = date3.Add(randomTimeSpan());
            germanDeck3.DateUpdated = date3.Add(randomTimeSpan());
            // Third German Deck's flash cards
            Card[] germanCards3 = createFlashCards(germanDeck3, "groß@big, tall|Klein@small, short|faul@lazy|fleißig@hard-working|teuer@expensive|günstig@inexpensive|schön@beautiful|hässlich@ugly|glücklich@happy|traurig@sad|Hell@Light, bright|dunkle@dark|leicht@easy; light (in weight)|schwer@heavy (weight), difficult|begeistert@excited, enthusiastic|enttäuscht@disappointed|mutig@brave, courageous|Stark@Strong|dumm@stupid|schlau@smart|optimistisch@optimistic|pessimistisch@pessimistic|großzügig@generous|geizig@stingy|sauber@clean, neat|schmutzig@dirty|introvertiert@introverted|Extrovertiert@extroverted|schüchtern@shy, timid|aufgeschlossen@outgoing|schnell@fast|langsam@slow|gut@good, well|schlecht@bad, badly|Modern@Modern|altmodisch@old-fashioned|heiß@hot|kalt@cold|freundlich@friendly, kind|wichtig@important|Alt@Old|jung@young|neu@new|spät@late|früh@early|lebhaft@lively|kurz@short|lang@long|schmal@narrow|breit@wide|niedrig@low|hoch@high|Still@Quiet|anders@different|gleich@same|berühmt@famous, well-known|lustig@funny, amusing|kommisch@strange; weird|ruhig@calm|geschäftig@busy, bustling|laut@loud|frei@free|leise@quiet|einfach@easy|stressig@stressful|klug@smart, intelligent",
                Language.German, Language.English, owner);
            // Associate the deck and category
            langCategory.Decks.Add(germanDeck3);
            germanDeck3.DeckCategories.Add(langCategory);

            // Add everything to the database and save
            context.DeckCategories.Add(langCategory);

            context.Decks.Add(germanDeck1);
            PositionInitializer.Init();
            foreach (Card card in germanCards1)
            {
                card.DeckPosition = PositionInitializer.Next();
                context.Cards.Add(card);
            }

            PositionInitializer.Init();
            context.Decks.Add(germanDeck2);
            foreach (Card card in germanCards2)
            {
                card.DeckPosition = PositionInitializer.Next();
                context.Cards.Add(card);
            }

            PositionInitializer.Init();
            context.Decks.Add(germanDeck3);
            foreach (Card card in germanCards3)
            {
                card.DeckPosition = PositionInitializer.Next();
                context.Cards.Add(card);
            }

            context.SaveChanges();
        }

        /**
         * Adds three decks to an EchoStudyDB that contain Spanish/English cards as well as a Spanish category.
         * The given EchoUser will be marked as the owner of everything initialized
         * Two of them are public, one of them are private
         */
        public static void addSpanishMaterial(EchoStudyDB context, EchoUser owner)
        {
            // Category for Spanish
            DeckCategory langCategory = new DeckCategory();
            langCategory.Decks = new List<Deck>();
            langCategory.Title = "Spanish";
            langCategory.Description = "Decks that are useful for learning Spanish";
            langCategory.UserId = owner.Id;
            langCategory.DateCreated = DateTime.Now;
            langCategory.DateTouched = DateTime.Now;
            langCategory.DateUpdated = DateTime.Now;

            // First Spanish Deck
            Deck spanishDeck1 = new Deck();
            spanishDeck1.DeckCategories = new List<DeckCategory>();
            spanishDeck1.Cards = new List<Card>();
            spanishDeck1.Title = "Easy Spanish Verbs";
            spanishDeck1.Description = "Introductory Spanish verbs helpful for people trying to learn Spanish";
            spanishDeck1.Access = Access.Public;
            spanishDeck1.DefaultFrontLang = Language.Spanish;
            spanishDeck1.DefaultBackLang = Language.English;
            spanishDeck1.UserId = owner.Id;
            DateTime date1 = DateTime.Now.Add(randomTimeSpan());
            spanishDeck1.DateCreated = date1;
            spanishDeck1.DateTouched = date1.Add(randomTimeSpan());
            spanishDeck1.DateUpdated = date1.Add(randomTimeSpan());
            // First Spanish Deck's flash cards
            Card[] spanishCards1 = createFlashCards(spanishDeck1, "amar@to love|andar@to walk|ayudar@to help|bailar@to dance|buscar@to look for|cantar@to sing|cenar@to have diner|contestar@to answer|comprar@to buy|cocinar@to cook|desayunar@to have breakfest|entrar en@to enter|escuchar@to listen to|esperar@to wait, to hope|esquiar@to ski|estudiar@to study|hablar@to speak|lavar@to wash|limpiar@to clean|llegar@to arrive|llevar@to wear|mirar@to watch|montar@to ride|nadar@to swim|necesitar@to need|organizar@to organize|pagar@to pay|practicar@to practice|preguntar@to ask|preparar@to prepare|sacar@to take out|terminar@to finish|tocar@to touch, play an instrument|tomar@to take|trabajar@to work|visitar@to visit|aprender@to learn|beber@to drink|comer@to eat|cometer@to make a mistake|comprender@to understand|correr@to run|creer@to believe|deber@to owe, should|leer@to read|poseer@to possess|romper@to break|temer@to fear|vender@to sell|abrir@to open|admitir@to admit|asistir@to attend|cubrir@to cover|decidir@to decide|describir@to describe|descubrir@to discover|discutir@to discuss|escribir@to write|existir@to exsist|permitir@to permit|recibir@to recieve|subir@to climb|sufrir@to suffer|unir@to unite|vivir@to live",
                Language.Spanish, Language.English, owner);
            // Associate the deck and category
            langCategory.Decks.Add(spanishDeck1);
            spanishDeck1.DeckCategories.Add(langCategory);

            // Second Spanish Deck
            Deck spanishDeck2 = new Deck();
            spanishDeck2.DeckCategories = new List<DeckCategory>();
            spanishDeck2.Cards = new List<Card>();
            spanishDeck2.Title = "Easy Spanish Nouns";
            spanishDeck2.Description = "Introductory Spanish nouns helpful for people trying to learn Spanish";
            spanishDeck2.Access = Access.Public;
            spanishDeck2.DefaultFrontLang = Language.Spanish;
            spanishDeck2.DefaultBackLang = Language.English;
            spanishDeck2.UserId = owner.Id;
            DateTime date2 = DateTime.Now.Add(randomTimeSpan());
            spanishDeck2.DateCreated = date2;
            spanishDeck2.DateTouched = date2.Add(randomTimeSpan());
            spanishDeck2.DateUpdated = DateTime.Now;
            // Second Spanish Deck's flash cards
            Card[] spanishCards2 = createFlashCards(spanishDeck2, "el supermercado@supermarket|la tienda@store|el día@day|el fin de semana@weekend|el domingo@Sunday|el jueves@Thursday|la película@movie|el número@number|el baloncesto@basketball|el béisbol@baseball|la cafetería@cafeteria|el chocolate@chocolate|la clase@class|el español@Spanish|el fútbol@soccer|el fútbol norteamericano@football|el jazz@jazz|la música@music|la pizza@pizza|la tarea@homework|el tenis@tennis|el voleibol@volleyball|el bolígrafo@pen|la calculadora@calculator|el lápiz@pencil|la librería@book store|el libro@book|el papel@paper|la mesa@table|el reloj@clock|la revista@magazine|la ropa@clothes|el dinero@money|hoy@today|mañana@tomorrow|las matemáticas@mathematics|la tarde@afternoon|el baile@dance|el concierto@concert|los deportes@sports|el examen@exam|el compañero@friend|el carro@car|el parque@park|la novela@novel|la teléfono@phone|el perro@dog|la guitarra@guitar|el helado@ice cream|la bicicleta@bicycle|el piano@piano|el restaurante@restaurant|el refresco@soft drink|la biblioteca@library|la casa@home|el centro@downtown|el cine@movie theater|la cocina@kitchen|la comida@food|el cuchillo@knife|cumpleaños@birthday|el desayuno@breakfast|el diciembre@December|el enero@January|la ensalada@salad|esposo@spouse|la estación@station or season|el febrero@February|la fiesta@party|la fruta@fruit|la hermana@sister|el hijo@son|el hombre@man|la hora@hour / time|inglés@English|el jefe@boss|los jueces@judges|julio@July|junio@June|la leche@milk|el limón@lemon|el lunes@Monday|la madre@mother|los maestros@teachers|la manzana@apple|el martes@Tuesday|marzo@March|mayo@May|el miércoles@Wednesday|un momento@moment|la mujer@woman|los mujeres@women|la naranja@orange|la niña@girl|el niño@boy|noche@night|novia@girlfriend|noviembre@November|novio@boyfriend|octubre@october|papá@dad|perdón@pardon|lo siento@sorry|la persona@person|el pescado@fish|la pimienta@pepper|el pollo@chicken|el sábado@Saturday|la sal@salt|septiembre@September|la sofá@sofa|la sopa@soup|el té@tea|las vacaciones@vacation|el vaso@drinking glass|viernes@Friday|el vino@wine|el huevo@egg|los pantalones@pants|la oficina@office|las camisas@shirts|la chaqueta@jacket|el/la jovén@young person|el boleto@ticket|el aeropuerto@airport|el pasaporte@passport|los dientes@teeth|la maleta@suitcase/bag|la farmacia@pharmacy|el enfermo@sick|el color@color|la vida@life|el amigo@friend|el idioma@language|la calle@street|el diente@tooth|el norte@north|el tenedor@fork|el sol@sun|la iglesia@church|el doctor@doctor|el joven@young person|la computadora@computer|el apartamento@apartment|el hotel@hotel|la camiseta@t-shirt|la blusa@blouse|el problema@problem|la ciudad@city|el estado@state|la vaca@cow|la falda@skirt|la familia@family|la fecha@date|la carta@letter|el gato@cat|los abuelos@grandparents|la semana@week|el hermano@brother|la hija@daughter|hijos@children|horas@hours|el cocinero@cook / chef|los colores@colors|los zapatos@shoes|el baño@bathroom|el vestido@dress|agosto@August|un trabajo@a job|el agua@water|los días@days|abril@April|la camisa@shirt|el profesor@professor / teacher|el sombrero@hat|¿Tienes tú@Do you have?|¿Necesitas tú@Do you need?|¿Quieres tu@Do you want?|¿Te gusta@Do you like?|Yo tengo@I have|Yo veo@I see|Yo necessito@I need|Yo quiero@I want|Me gusta@I like|la pared@wall|el chico@boy|la chica@girl|la tinta@ink|la amiga@female friend|el estudiante@student|la escuela@school|la pagina@page|el móvil@cellphone|el queso@cheese|la carne@meat|la hamburguesa@hamburger|el pan@bread|el tren@train|la estación de tren@train station|el autobús@bus|la musica@music|la fotografía@photograph|la foto@photo|la gasolina@gasoline|la medicina@medicine|el automóvil@automobile|el coche@car|un café@coffee shop|el teléfono celular@cell phone|el teatro@theater|la universidad@university|el banco@bank|la muchacha@girl|el padre@father",
                Language.Spanish, Language.English, owner);
            // Associate the deck and category
            langCategory.Decks.Add(spanishDeck2);
            spanishDeck2.DeckCategories.Add(langCategory);

            // Third Spanish Deck
            Deck spanishDeck3 = new Deck();
            spanishDeck3.DeckCategories = new List<DeckCategory>();
            spanishDeck3.Cards = new List<Card>();
            spanishDeck3.Title = "Easy Spanish Adjectives";
            spanishDeck3.Description = "Introductory Spanish adjectives helpful for people trying to learn Spanish";
            spanishDeck3.Access = Access.Private;
            spanishDeck3.DefaultFrontLang = Language.English;
            spanishDeck3.DefaultBackLang = Language.Spanish;
            spanishDeck3.UserId = owner.Id;
            DateTime date3 = DateTime.Now.Add(randomTimeSpan());
            spanishDeck3.DateCreated = date3;
            spanishDeck3.DateTouched = date3.Add(randomTimeSpan());
            spanishDeck3.DateUpdated = date3.Add(randomTimeSpan());
            // Third Spanish Deck's flash cards
            Card[] spanishCards3 = createFlashCards(spanishDeck3, "Long@Largo|Short@Corto|Big@Grande|Small@Pequeño|Tall@Alto|Beautiful@Guapa|Clean@Limpio|Dirty@Sucio|Smart@Elegante / inteligente|Skinny@Muy flaco|Thick@Espeso / grueso|Thin@Delgado / fino|Handsome@Guapo (chico)|Old fashioned@Anticuado|Ugly@Feo|Nice@Agradable / bueno / simpático|Good@Bueno|Bad@Malo|Alive@Vivo|Dead@Muerto|Careful@Cuidadoso|Clever@Astuto / listo|Easy@Fácil|Difficult@Difícil|Famous@Famoso|Unimportant@Sin Importancia|Important@Importante|Soft@Blando / suave|Hard@Duro / difícil|Funny@gracioso|Fun@Que gusta|Powerful@poderoso|Rich@Rico|Poor@Pobre|Shy@Tímido|Tender@Tierno|Uninterested@Desinteresado|Wrong@Equivocado|Right@Correcto|Red@Rojo|Orange@Naranja|Yellow@Amarillo|Green@Verde|Blue@Azul|Purple@Morado|Gray (Am) - Grey (Br)@Gris|Black@Negro|White@Blanco|Horrible@Horrible|Nasty@Repugnante / Horrible|Terrible@Terrible|Excellent@Excelente|Angry@Enfadado|Clumsy@Torpe|Embarrassed@Avergonzado|Jealous@Celoso|Lazy@Perezoso|Mysterious@Misterioso|Nervous@Nervioso|Scary@Que da miedo|Worried@Preocupado|Brave@Valiente|Calm@Calmado / traquilo|Faithful@Fiel / Leal|Sad@Triste|Happy@Feliz|Kind@Amable|Proud@Orgulloso|Silly@Tonto|Thankful@Agradecido|Fast@Rápido|Slow@Lento|Expensive@Caro|Cheap@Barato|Narrow@Estrecho|Wide@Ancho|Loud@Ruidoso|Quiet@Tranquilo / silencioso|Intelligent@Inteligente|Stupid@Estúpido|Wet@Mojado|Dry@Seco|Heavy@Pesado|Light@Ligero|Deep@Profundo|Weak@Débil|Strong@Fuerte|Young@Joven|New@Nuevo|Old@Viejo|High@Alto|Low@Bajo|Generous@Generoso|Mean@Tacaño|True@Verdadero|False@Falso|Safe@Seguro|Dangerous@Peligroso|Early@Temprano|Late@Tarde|Light@Luminoso / ligero|Dark@Oscuro|Open@Abierto|Closed@Cerrado|Tight@Estrecho - apretado|Loose@Holgado / suelto|Full@Lleno|Empty@Vacío|Hot@Caliente|Cold@Frío|Interesting@Interesante|Boring@Aburrido|Lucky@Afortunado|Unlucky@Desafortunado|Far@Lejos|Near@Cerca|Pleasant@Agradable|Unpleasant@Desagradable|Fair@Justo / limpio (juego)|Unfair@Injusto / sucio (juego)|Normal@Normal|Gentle@Amable|Adventurous@Aventurero|Sharp@Afilado|Bright@Brillante|Curious@Curioso|Dependent@Dependiente|Depressed@Deprimido|Acid@Ácido|Bitter@Agrio|Clear@Claro / despejado|Cruel@Cruel|Delicate@Delicado|Fat@Gordo|Great@Estupendo / buenísimo / genial|Healthy@Sano|Huge@Enorme|Ill@Enfermo|Natural@Natural|Necessary@Necesario|Opposite@Contrario|Physical@Físico|Quick@Rápido|Ready@Preparado|Rough@Áspero|Serious@Serio|Spicy@Picante|Sticky@Pegajoso|Straight@Recto / Directo|Strange@Extraño|Sudden@Repentino|Tired@Cansado|Violent@Violento|Warm@Cálido|Wide@Ancho|Alone@Solo|Brown@Marrón|Busy@Ocupado|Dumb@Tonto|Free@Libre - gratis|Funny@Gracioso|Good looking@Guapo|Inferior@Inferior|Large@Grande|Polite@Educado / formal|Unpolite@Poco educado / informal|Pretty@Guapa (chica)|Resistant@Resistente|Sick@Enfermo|Superior@Superior|Sure@Seguro|Advanced@Avanzado|Afraid@Temeroso|Aggressive@Agresivo|Ancient@Antiguo|Annoying@Molesto / Irritante|Anonymous@Anónimo|Attractive@Atractivo|Awesome@Impresionante|Awful@Horrible|Basic@Básico|Blind@Ciego|Chatty@Hablador|Comfortable@Cómodo|Cool@chulo/guay|Crispy@Crujiente|Crowded@Abarrotado|Delicious@Delicioso|Educated@Culto|Energetic@Lleno de energía|Enormous@Enorme|Enough@Suficiente|Entertaining@Entretenido|Enthusiastic@Entusiasta|Excited@Entusiasmado|Exciting@Emocionante|Exhausted@Agotado|Fabulous@Fabuloso|Fake@Falso|Fashionable@De moda|Fit@En forma|Fluent@Fluido|Honest@Honesto / Sincero|Hungry@Hambriento|Illegal@Ilegal|Interested@Interesado|Naughty@Travieso|Noisy@Ruidoso|Optimistic@Optimista|Peaceful@Pacífico|Posh@Pijo|Royal@Real|Selfish@Egoísta|Sensible@Sensato|Sensitive@Sensible|Smelly@Que huele mal|Stressful@Estresante|Surprising@Sorprendente|Unemployed@Desempleado|Useful@Útil|Useless@Inútil|Enjoyable@Placentero|Smooth@Delicado|Different@Diferente|Sweet@Dulce|Special@Especial|Physical@Físico|Little@Pequeño|Tiny@Muy pequeño - diminuto|Huge@Enorme|Flat@Plano / Llano|Possible@Posible|Private@Privado|Enormous@Enorme|Solid@Sólido|Simple@Simple / Sencillo|Careless@Descuidado",
                Language.English, Language.Spanish, owner);
            // Associate the deck and category
            langCategory.Decks.Add(spanishDeck3);
            spanishDeck3.DeckCategories.Add(langCategory);

            // Add everything to the database and save
            context.DeckCategories.Add(langCategory);
            context.Decks.Add(spanishDeck1);

            PositionInitializer.Init();
            foreach (Card card in spanishCards1)
            {
                card.DeckPosition = PositionInitializer.Next();
                context.Cards.Add(card);
            }
            context.Decks.Add(spanishDeck2);

            PositionInitializer.Init();
            foreach (Card card in spanishCards2)
            {
                card.DeckPosition = PositionInitializer.Next();
                context.Cards.Add(card);
            }
            context.Decks.Add(spanishDeck3);

            PositionInitializer.Init();
            foreach (Card card in spanishCards3)
            {
                card.DeckPosition = PositionInitializer.Next();
                context.Cards.Add(card);
            }
            context.SaveChanges();
        }

        /**
         * Given a deck and a string of a bunch of cards (front/back separated by an @, cards separated by a ;), create a bunch of cards for the deck
         */
        public static Card[] createFlashCards(Deck deck, string cardsStringForm, Language frontLang, Language backLang, EchoUser owner)
        {
            // Spanish flash cards
            string[] cardsStrings = cardsStringForm.Split('|');
            Card[] cards = new Card[cardsStrings.Length];

            // Create the cards and add them to the deck
            for (int i = 0; i < cardsStrings.Length; i++)
            {
                string[] splitCard = cardsStrings[i].Split('@');
                Card card = new Card();
                card.FrontText = splitCard[0].Trim();
                card.BackText = splitCard[1].Trim();
                card.FrontLang = frontLang;
                card.BackLang = backLang;
                card.FrontAudio = AmazonUploader.getFileName(card.FrontText, card.FrontLang);
                card.BackAudio = AmazonUploader.getFileName(card.BackText, card.BackLang);
                card.Score = 0;
                card.UserId = owner.Id;
                DateTime date = DateTime.Now.Add(randomTimeSpan());
                card.DateCreated = date;
                card.DateTouched = date.Add(randomTimeSpan());
                card.DateUpdated = date.Add(randomTimeSpan());
                card.Deck = deck;
                card.DeckID = deck.DeckID;
                deck.Cards.Add(card);
                cards[i] = card;
            }

            return cards;
        }

        /**
         * Returns a random time span from 0 to 2 days
         */
        public static TimeSpan randomTimeSpan()
        {
            Random random = new Random();
            TimeSpan randomTime = new TimeSpan(random.Next(1),random.Next(23), random.Next(59), random.Next(59));
            return randomTime;
        }

        internal static class PositionInitializer
        {
            private static string currentPos = "";

            public static void Init()
            {
                currentPos = "";
            }

            public static string Next()
            {
                if (currentPos.Length == 0) return (currentPos = "ia");

                int posLen = currentPos.Length;
                string retPos;

                for(int i = posLen - 1; i >= 0; i--)
                {
                    if(currentPos[i] != 'z')
                    {
                        retPos = currentPos[..i];
                        retPos += (char)(currentPos[i] + 1);
                        if (i < posLen - 1)
                        {
                            retPos += currentPos[(i + 2)..];
                            for(int j = i+1; j < posLen; j++)
                            {
                                retPos += 'a';
                            }
                        }
                        currentPos = retPos;
                        return retPos;
                    }
                }

                currentPos = "";
                for(int i = 0; i <= posLen; i++)
                {
                    currentPos += 'a';
                }

                return currentPos;
            }
        }
    }

    /**
 * Initializes 3 roles by checking if they're there and creating them if they're not there
 * Users are then initialized in the same fashion and are assigned roles depending on their email
 */
    public class IdentityInitializer
    {
        public static async Task Initialize(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var identityContext = services.GetService<EchoStudyDB>();

                string[] roles = new string[] { "Administrator", "Standard" };

                // Ensure our roles exist
                foreach (string role in roles)
                {
                    var roleStore = new RoleStore<IdentityRole>(identityContext);

                    if (!identityContext.Roles.Any(r => r.Name == role))
                    {
                        var nr = new IdentityRole(role);
                        nr.NormalizedName = role.ToUpper();
                        await roleStore.CreateAsync(nr);
                    }
                }

                // All users
                var user1 = new EchoUser
                {
                    Email = "admin@echoStudy.com",
                    NormalizedEmail = "ADMIN@ECHOSTUDY.COM",
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    PhoneNumber = "123-456-7890",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };

                var user2 = new EchoUser
                {
                    Email = "JohnDoe@gmail.com",
                    NormalizedEmail = "JOHNDOE@GMAIL.COM",
                    UserName = "JohnDoe",
                    NormalizedUserName = "JOHNDOE",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    PhoneNumber = "123-456-7890",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };

                var user3 = new EchoUser
                {
                    Email = "JaneDoe@gmail.com",
                    NormalizedEmail = "JANEDOE@GMAIL.COM",
                    UserName = "JaneDoe",
                    NormalizedUserName = "JANEDOE",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    PhoneNumber = "123-456-7890",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };

                var user4 = new EchoUser
                {
                    Email = "JohnSmith@gmail.com",
                    NormalizedEmail = "JOHNSMITH@GMAIL.COM",
                    UserName = "JohnSmith",
                    NormalizedUserName = "JOHNSMITH",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    PhoneNumber = "123-456-7890",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };

                var user5 = new EchoUser
                {
                    Email = "MarySmith@gmail.com",
                    NormalizedEmail = "MARYSMITH@GMAIL.COM",
                    UserName = "MarySmith",
                    NormalizedUserName = "MARYSMITH",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    PhoneNumber = "123-456-7890",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };

                var user6 = new EchoUser
                {
                    Email = "GeorgeBrown@gmail.com",
                    NormalizedEmail = "GEORGEBROWN@GMAIL.COM",
                    UserName = "GeorgeBrown",
                    NormalizedUserName = "GEORGEBROWN",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    PhoneNumber = "123-456-7890",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };

                var user7 = new EchoUser
                {
                    Email = "SarahBrown@gmail.com",
                    NormalizedEmail = "SARAHBROWN@GMAIL.COM",
                    UserName = "SarahBrown",
                    NormalizedUserName = "SARAHBROWN",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    PhoneNumber = "123-456-7890",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };

                EchoUser[] users = new EchoUser[7] { user1, user2, user3, user4, user5, user6, user7 };

                // Create each user if they don't already exist
                foreach (EchoUser user in users)
                {
                    if (!identityContext.Users.Any(u => u.UserName == user.UserName))
                    {
                        var password = new PasswordHasher<EchoUser>();
                        var hashed = password.HashPassword(user, "123ABC!@#def");
                        user.PasswordHash = hashed;

                        var userStore = new UserStore<EchoUser>(identityContext);
                        await userStore.CreateAsync(user);
                        if (user.Email.StartsWith("admin"))
                        {
                            await AssignRoles(services, user.Email, new string[1] { "ADMINISTRATOR" });
                        }
                        else
                        {
                            await AssignRoles(services, user.Email, new string[1] { "STANDARD" });
                        }
                    }
                }

                // Save
                await identityContext.SaveChangesAsync();
            }
        }

        /**
         * Helper method that assigns provided roles to the provided email.
         */
        public static async Task<IdentityResult> AssignRoles(IServiceProvider services, string email, string[] roles)
        {
            var _userManager = services.GetService<UserManager<EchoUser>>();
            EchoUser user = await _userManager.FindByEmailAsync(email);
            var result = await _userManager.AddToRolesAsync(user, roles);
            return result;
        }
    }
}
