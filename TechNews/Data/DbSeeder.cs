using Microsoft.AspNetCore.Identity;
using TechNews.Models;
using TechNews.Data;

namespace TechNews.Data
{
    public static class DbSeeder
    {
        public static async Task SeedData(IServiceProvider serviceProvider, NewsContext context, UserManager<IdentityUser> userManager)
        {
            // Головні адміни (Тільки двоє)
            var admins = new List<string> { "admin@technews.com", "super_admin@technews.com" };
            foreach (var email in admins)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                    await userManager.CreateAsync(user, "Password123!");
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            // Редактори (Публікують новини)
            var editors = new List<string> { "editor_tech@technews.com", "editor_it@technews.com", "editor_gadget@technews.com", "editor_ai@technews.com" };
            foreach (var email in editors)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                    await userManager.CreateAsync(user, "Password123!");
                    await userManager.AddToRoleAsync(user, "Editor");
                }
            }

            // Звичайні користувачі (Коментують)
            var users = new List<(string Email, string Name)>
            {
                ("alex_dev@gmail.com", "AlexDev"),
                ("maria_design@gmail.com", "MariaDesign"),
                ("cyber_fan@gmail.com", "CyberPunk2077"),
                ("pro_gamer@gmail.com", "ProGamerUA"),
                ("qa_ninja@gmail.com", "QA_Ninja")
            };
            foreach (var u in users)
            {
                if (await userManager.FindByEmailAsync(u.Email) == null)
                {
                    var user = new IdentityUser { UserName = u.Email, Email = u.Email, EmailConfirmed = true };
                    await userManager.CreateAsync(user, "UserPassword1!"); 
                }
            }

            // Перевірка на наявність новин, щоб не дублювати
            if (context.Posts.Any()) return;

            // Великий список новин
            var posts = new List<Post>
            {
                // ТЕХНОЛОГІЇ (Category 1)
                new Post
                {
                    Title = "OpenAI представила GPT-5 Turbo: ще швидше, розумніші відповіді",
                    CategoryId = 1,
                    ShortDescription = "Нова модель обіцяє скорочення витрат у 3 рази та кращу якість генерації.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/4/4d/OpenAI_Logo.svg",
                    Content = @"<p><strong>OpenAI</strong> офіційно анонсувала модель GPT-5 Turbo.</p>
                                <ul>
                                    <li>Швидкість збільшена на 40%.</li>
                                    <li>Контекстне вікно — до 2 млн токенів.</li>
                                    <li>Покращена фактичність та знижена галюцинація.</li>
                                </ul>
                                <p>Модель вже доступна у API.</p>",
                    CreatedAt = DateTime.Now.AddDays(-4),
                    AuthorEmail = "editor_ai@technews.com"
                },
                new Post
                {
                    Title = "Google презентував Android 16: новий рівень персоналізації та ШІ",
                    CategoryId = 1,
                    ShortDescription = "Оновлення отримало функцію повністю адаптивних інтерфейсів.",
                    ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Ftse4.mm.bing.net%2Fth%2Fid%2FOIP.cv9CYn1CVPiRhVh3kF5_fQHaEc%3Fpid%3DApi&f=1&ipt=5a111601887f16c1e8cce01f8df74d587651e8333d551ac31593c86aa2cb2ecf",
                    Content = @"<p>Android 16 значно розширює можливості вбудованого AI.</p>
                                <p>Функції:</p>
                                <ul>
                                    <li>Генеративні шпалери 2.0.</li>
                                    <li>Смарт-пам’ять процесів.</li>
                                    <li>До 25% швидший рендер UI.</li>
                                </ul>",
                    CreatedAt = DateTime.Now.AddDays(-3),
                    AuthorEmail = "editor_tech@technews.com"
                },
                new Post
                {
                    Title = "Офіційно представлено Wi-Fi 8: швидкість до 60 Гбіт/с",
                    CategoryId = 1,
                    ShortDescription = "Стандарт сфокусований на потоках AI-обчислень та AR-пристроях.",
                    ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fwww.tomsguide.fr%2Fcontent%2Fuploads%2Fsites%2F2%2F2024%2F07%2Fquest-ce-que-le-wifi-8-jpg.jpg&f=1&nofb=1&ipt=2314e1eeb87faa83bd2959015b26c090f5cdbcc3c4d778a0a701fb1066054749",
                    Content = @"<p>Wi-Fi 8 отримав підтримку субтерагерцевого діапазону.</p>
                                <p>Це відкриває можливості для AR-гарнітур та високонавантажених дата-центрів.</p>",
                    CreatedAt = DateTime.Now.AddDays(-7),
                    AuthorEmail = "editor_tech@technews.com"
                },
                new Post
                {
                    Title = "NVIDIA анонсувала серію RTX 5000: революція в графіці",
                    CategoryId = 1, 
                    ShortDescription = "Інсайдери розкрили характеристики нового покоління відеокарт. Очікується приріст продуктивності на 40%.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a4/NVIDIA_logo.svg/1200px-NVIDIA_logo.svg.png",
                    Content = @"<p>За останніми витоками інформації, <strong>NVIDIA</strong> готується представити нову архітектуру Blackwell вже наприкінці цього року.</p>
                                <h3>Ключові особливості:</h3>
                                <ul>
                                    <li>Новий техпроцес 3нм від TSMC.</li>
                                    <li>Підтримка GDDR7 пам'яті з пропускною здатністю до 32 Гбіт/с.</li>
                                    <li>Енергоефективність, що на 30% вища за серію RTX 40.</li>
                                </ul>
                                <p>Флагманська модель RTX 5090, ймовірно, отримає 32 ГБ відеопам'яті та шину 512 біт. Це зробить її абсолютним лідером для задач ШІ та 8K-геймінгу.</p>
                                <blockquote>""Це найбільший стрибок продуктивності з часів Pascal"", — зазначають аналітики.</blockquote>",
                    CreatedAt = DateTime.Now.AddDays(-10),
                    AuthorEmail = "editor_tech@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "pro_gamer@gmail.com", Content = "Ціна буде космос, доведеться нирку продавати...", CreatedAt = DateTime.Now.AddDays(-10).AddHours(2) },
                        new Comment { AuthorEmail = "cyber_fan@gmail.com", Content = "Чекаю, щоб оновити свою 3060. Сподіваюсь, БЖ на 850Вт вистачить.", CreatedAt = DateTime.Now.AddDays(-9) }
                    }
                },
                new Post
                {
                    Title = "Starlink досяг швидкості 1 Гбіт/с у тестах",
                    CategoryId = 1,
                    ShortDescription = "Супутниковий інтернет від SpaceX виходить на новий рівень швидкості та стабільності.",
                    ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fcdn.freelogovectors.net%2Fwp-content%2Fuploads%2F2021%2F02%2Fstarlink-logo-freelogovectors.net_.png&f=1&nofb=1&ipt=c0b51ea6d3bceb3b6c6aca3886543005dad8554433cadf4dce6ff7a6eab2ca34",
                    Content = @"<p>Користувачі в США повідомляють про рекордні швидкості завантаження через термінали Starlink нового покоління.</p>
                                <p>Завдяки запуску супутників V2 mini, пропускна здатність мережі значно зросла. Це відкриває двері для:</p>
                                <ul>
                                    <li>Хмарного геймінгу у віддалених регіонах.</li>
                                    <li>Стрімінгу 8K відео без затримок.</li>
                                    <li>Надійного зв'язку для бізнесу.</li>
                                </ul>",
                    CreatedAt = DateTime.Now.AddDays(-8),
                    AuthorEmail = "editor_tech@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "alex_dev@gmail.com", Content = "Для села це просто порятунок.", CreatedAt = DateTime.Now.AddDays(-7) }
                    }
                },
                new Post
                {
                    Title = "Intel показала Core Ultra 200: нейропроцесор у кожному ноутбуці",
                    CategoryId = 1,
                    ShortDescription = "Нова лінійка Meteor Lake Refresh отримує вдосконалений NPU 2.0 для прискорення AI-задач.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Intel-logo-2022.png/800px-Intel-logo-2022.png",
                    Content = @"<p>Компанія <strong>Intel</strong> презентувала нову серію Core Ultra 200, орієнтовану на штучний інтелект.</p>
                                 <h3>Основні фішки:</h3>
                                 <ul>
                                     <li>Вбудований NPU 2.0 з продуктивністю до 45 TOPS.</li>
                                     <li>Покращена графіка Xe 3-го покоління.</li>
                                     <li>Зниження енергоспоживання на 20%.</li>
                                 </ul>
                                 <p>Ноутбуки на базі нових чипів вийдуть вже у першому кварталі 2025 року.</p>",
                    CreatedAt = DateTime.Now.AddDays(-7),
                    AuthorEmail = "editor_tech@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "qa_ninja@gmail.com", Content = "Цікаво, чи зможе це замінити відеокарту для нейронок?", CreatedAt = DateTime.Now.AddDays(-7).AddHours(3) },
                        new Comment { AuthorEmail = "alex_dev@gmail.com", Content = "Intel оживає, нарешті!", CreatedAt = DateTime.Now.AddDays(-6).AddHours(1) }
                    }
                },
                new Post
                {
                    Title = "Tesla Cybertruck отримав режим «Off-Road Max»",
                    CategoryId = 1,
                    ShortDescription = "Нове оновлення значно покращує поведінку на бездоріжжі.",
                    ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fwww.freepnglogos.com%2Fuploads%2Ftesla-logo-png-24.png&f=1&nofb=1&ipt=a02a511dc50cd8d3e25065e438a5ab8dc83a971dce3b5151a2cf38cdbaad14ae",
                    Content = @"<p>Останнє оновлення ПО для <strong>Tesla Cybertruck</strong> активувало режим Off-Road Max.</p>
                                 <p>Він включає:</p>
                                 <ul>
                                     <li>Підвищення кліренсу до 45 см.</li>
                                     <li>Покращений контроль тяги.</li>
                                     <li>Режим повільного спуску з пагорбів.</li>
                                 </ul>
                                 <p>Оновлення доступне всім власникам через OTA.</p>",
                    CreatedAt = DateTime.Now.AddDays(-6),
                    AuthorEmail = "editor_tech@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "cyber_fan@gmail.com", Content = "А тепер ще й танковий розворот зробіть!", CreatedAt = DateTime.Now.AddDays(-5) }
                    }
                },
                new Post
                {
                    Title = "AMD Ryzen 9800X3D обіцяє +25% FPS у 4K",
                    CategoryId = 1,
                    ShortDescription = "Гібридний 3D-кеш нового покоління.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/7c/AMD_Logo.svg/800px-AMD_Logo.svg.png",
                    Content = @"<p>Нові тести показують, що Ryzen 9800X3D перевершує навіть топові Intel у важких іграх.</p>
                                 <p>Покращений 3D V-Cache дає приріст до 25% FPS у 4K.</p>",
                    CreatedAt = DateTime.Now.AddDays(-8),
                    AuthorEmail = "editor_tech@technews.com",
                },

                // ПРОГРАМУВАННЯ (Category 2)
                new Post
                {
                    Title = "TypeScript 6.0 представив революційну систему типів",
                    CategoryId = 2,
                    ShortDescription = "Версія 6.0 додає сигнатури-композиції та нові generic-механізми.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/4/4c/Typescript_logo_2020.svg",
                    Content = @"<p>TypeScript продовжує домінувати у фронтенд-екосистемі.</p>
                                <p>Основні зміни:</p>
                                <ul>
                                    <li>Композиційні сигнатури функцій.</li>
                                    <li>Покращені union-типи.</li>
                                    <li>Новий режим strictAsync.</li>
                                </ul>",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    AuthorEmail = "editor_it@technews.com"
                },
                new Post
                {
                    Title = "GitHub Copilot 3.0 навчився працювати повністю офлайн",
                    CategoryId = 2,
                    ShortDescription = "Нова функція дозволяє запускати локальну LLM без інтернету.",
                    ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fausum.cloud%2Fwp-content%2Fuploads%2F2024%2F01%2Fgithub-copilot-microsoft-ausum-cloud.png&f=1&nofb=1&ipt=e719079cff7c6de08d45376902c54322d099838f8850215078d47bcf1adcb208",
                    Content = @"<p>GitHub представив Copilot 3.0 з можливістю офлайн-роботи.</p>
                                <p>Для цього використовується локальна LLM, оптимізована під GPU ноутбуків.</p>",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    AuthorEmail = "editor_ai@technews.com"
                },
                new Post
                {
                    Title = "Python 3.13: Ера без GIL настає",
                    CategoryId = 2,
                    ShortDescription = "Революційне оновлення мови Python обіцяє справжню багатопотоковість.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c3/Python-logo-notext.svg/800px-Python-logo-notext.svg.png",
                    Content = @"<p>Розробники Python офіційно затвердили план по відмові від <strong>Global Interpreter Lock (GIL)</strong>. Це історична подія для мови.</p>
                                <p>Раніше Python не міг ефективно використовувати всі ядра процесора в одному процесі. Версія 3.13 вводить експериментальний режим <em>no-GIL</em>.</p>
                                <h3>Що це змінює?</h3>
                                <p>Це дозволить Python конкурувати з Go та Java у високошвидкісних обчисленнях та ML-задачах без необхідності писати код на C++.</p>",
                    CreatedAt = DateTime.Now.AddDays(-6),
                    AuthorEmail = "editor_it@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "alex_dev@gmail.com", Content = "Нарешті! Чекав цього 10 років.", CreatedAt = DateTime.Now.AddDays(-6).AddHours(5) },
                        new Comment { AuthorEmail = "maria_design@gmail.com", Content = "Для Data Science це просто маст-хев оновлення.", CreatedAt = DateTime.Now.AddDays(-5) }
                    }
                },
                new Post
                {
                    Title = "Чому Rust стає стандартом для системного програмування",
                    CategoryId = 2,
                    ShortDescription = "Microsoft та Linux Foundation активно переписують критичні компоненти на Rust.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d5/Rust_programming_language_black_logo.svg/1024px-Rust_programming_language_black_logo.svg.png",
                    Content = @"<p>Безпека пам'яті стала головним пріоритетом індустрії. <strong>Rust</strong> гарантує це на етапі компіляції.</p>
                                <p>Вже зараз ядро Linux 6.1 офіційно підтримує драйвери на Rust, а Windows 11 включає компоненти, переписані з C++ на Rust для зменшення кількості вразливостей.</p>
                                <p>Чи варто вчити Rust у 2025? Однозначно так.</p>",
                    CreatedAt = DateTime.Now.AddDays(-4),
                    AuthorEmail = "editor_it@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "qa_ninja@gmail.com", Content = "Крива навчання крута, але воно того варте.", CreatedAt = DateTime.Now.AddDays(-3) }
                    }
                },
                new Post
                {
                    Title = "Node.js 23 переходить на WASM-движок",
                    CategoryId = 2,
                    ShortDescription = "Новий механізм дозволяє виконувати модулі WebAssembly без додаткових обгорток.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/d/d9/Node.js_logo.svg",
                    Content = @"<p>Команда <strong>Node.js</strong> оголосила про інтеграцію нового WASM-движка.</p>
                                <p>Він забезпечує:</p>
                                <ul>
                                    <li>Пряме виконання модулів без нативних аддонів.</li>
                                    <li>Вищу безпеку завдяки sandboxing.</li>
                                    <li>Прискорення критичних обчислень у 3-5 разів.</li>
                                </ul>",
                    CreatedAt = DateTime.Now.AddDays(-5),
                    AuthorEmail = "editor_it@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "alex_dev@gmail.com", Content = "Ну нарешті! Чекав WASM у Node років 4.", CreatedAt = DateTime.Now.AddDays(-5).AddHours(2) }
                    }
                },
                new Post
                {
                    Title = "Django 5.1 приносить асинхронні ORM-операції",
                    CategoryId = 2,
                    ShortDescription = "Вперше ORM Django отримує повноцінний async API.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/75/Django_logo.svg/640px-Django_logo.svg.png",
                    Content = @"<p>Django офіційно включив асинхронні операції ORM у версію 5.1.</p>
                                <ul>
                                    <li><code>await User.objects.aget()</code></li>
                                    <li>Підтримка async транзакцій</li>
                                    <li>Покращення продуктивності під високими навантаженнями</li>
                                </ul>
                                <p>Фреймворк стає сучаснішим, не втрачаючи стабільності.</p>",
                    CreatedAt = DateTime.Now.AddDays(-3),
                    AuthorEmail = "editor_it@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "maria_design@gmail.com", Content = "Ого, тепер Django може конкурувати з FastAPI.", CreatedAt = DateTime.Now.AddDays(-2) }
                    }
                },

                // ГАДЖЕТИ (Category 3)
                new Post
                {
                    Title = "Xiaomi Band 9 отримав новий OLED 2.0 дисплей",
                    CategoryId = 3,
                    ShortDescription = "Набагато яскравіший екран та 20 днів автономності.",
                    ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fi02.appmifile.com%2Fmi-com-product%2Ffly-birds%2Fxiaomi-smart-band-9-active%2F9c9c102aba50877c656cbf71eaf7923e.png&f=1&nofb=1&ipt=f72ba6c3b24285cfb3c4cdeec3c227dd5b604f1e7b79dca39e8bbdd6b4996477",
                    Content = @"<p>Новий Xiaomi Band 9 зберіг низьку ціну, але суттєво оновив екран.</p>
                                <ul>
                                    <li>Яскравість — 1500 ніт.</li>
                                    <li>Автономність — до 20 днів.</li>
                                    <li>Підтримка Always-On Display.</li>
                                </ul>",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    AuthorEmail = "editor_gadget@technews.com"
                },
                new Post
                {
                    Title = "Apple Vision Pro: Провал чи майбутнє?",
                    CategoryId = 3,
                    ShortDescription = "Через місяць після релізу користувачі масово повертають гарнітуру. Розбираємося в причинах.",
                    ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fwww.apple.com%2Fnewsroom%2Fimages%2Fmedia%2Fintroducing-apple-vision-pro%2FApple-WWDC23-Vision-Pro-glass-230605_big.jpg.large_2x.jpg&f=1&nofb=1&ipt=cf1be7b17bfdc2879330f6c3eabdf25d1c27eace4b70e8409532ad6d90eb618f",
                    Content = @"<p>Гарнітура змішаної реальності від Apple викликала вау-ефект, але реальність виявилася суворішою.</p>
                                <h3>Основні скарги:</h3>
                                <ol>
                                    <li><strong>Вага:</strong> Пристрій занадто важкий для тривалого носіння.</li>
                                    <li><strong>Ціна:</strong> $3500 – це занадто для ""іграшки"".</li>
                                    <li><strong>Мало контенту:</strong> Відсутність killer-app.</li>
                                </ol>
                                <p>Проте, інженери кажуть, що це лише dev-kit для ентузіастів, а справжній масовий продукт вийде через 2-3 роки.</p>",
                    CreatedAt = DateTime.Now.AddDays(-5),
                    AuthorEmail = "editor_gadget@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "cyber_fan@gmail.com", Content = "Технологія крута, але ергономіка жахлива.", CreatedAt = DateTime.Now.AddDays(-4) },
                        new Comment { AuthorEmail = "alex_dev@gmail.com", Content = "Це як перший iPhone - недосконалий, але революційний.", CreatedAt = DateTime.Now.AddDays(-4).AddHours(1) }
                    }
                },
                new Post
                {
                    Title = "Samsung Galaxy S25 Ultra: Перші рендери",
                    CategoryId = 3,
                    ShortDescription = "Новий дизайн камер та титановий корпус. Що відомо про майбутній флагман?",
                    ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Flatestlogo.com%2Fwp-content%2Fuploads%2F2024%2F01%2Fsamsung-logo.png&f=1&nofb=1&ipt=16b68ec5f08371e1d98857ad4b436631d8d3c77f4fe1271b6cd01205cb487004",
                    Content = @"<p>Відомий інсайдер OnLeaks опублікував рендери майбутнього флагмана Samsung.</p>
                                <p>Очікується повернення до більш заокруглених граней для кращої ергономіки та використання нового скла Gorilla Glass Armor 2.</p>
                                <p>Камери отримають нові сенсори на 200 Мп з покращеною нічною зйомкою.</p>",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    AuthorEmail = "editor_gadget@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "maria_design@gmail.com", Content = "Сподіваюсь, вони нарешті приберуть ШІМ екрану.", CreatedAt = DateTime.Now.AddDays(-1) }
                    }
                },
                new Post
                {
                    Title = "Nothing Phone (3): прозорий дизайн і Snapdragon 8s Gen 3",
                    CategoryId = 3,
                    ShortDescription = "Витік підтверджує нову світлодіодну систему Glyph.",
                    ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fvectorseek.com%2Fwp-content%2Fuploads%2F2023%2F11%2FNothing-records-Logo-Vector.svg-.png&f=1&nofb=1&ipt=a683f6621161d7ae87e2d13b631e8e9bec1b0cff4847c0502aa3fb86cbf4542d",
                    Content = @"<p>Бренд <strong>Nothing</strong> готує до релізу Phone (3).</p>
                                 <p>Головні зміни:</p>
                                 <ul>
                                     <li>Покращена система сповіщень через Glyph 2.0.</li>
                                     <li>Процесор Snapdragon 8s Gen 3.</li>
                                     <li>Новий мінімалістичний UI.</li>
                                 </ul>",
                    CreatedAt = DateTime.Now.AddDays(-4),
                    AuthorEmail = "editor_gadget@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "cyber_fan@gmail.com", Content = "Nothing повертається в гру!", CreatedAt = DateTime.Now.AddDays(-4).AddHours(4) }
                    }
                },
                new Post
                {
                    Title = "Xiaomi Band 9: підтримка eSIM і GPS",
                    CategoryId = 3,
                    ShortDescription = "Нарешті фітнес-браслет отримує повноцінні телефонні можливості.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/29/Xiaomi_logo.svg/800px-Xiaomi_logo.svg.png",
                    Content = @"<p>Компанія <strong>Xiaomi</strong> готує новий Band 9.</p>
                                <p>Інсайдери повідомляють про:</p>
                                <ul>
                                    <li>Підтримку eSIM.</li>
                                    <li>Вбудований GPS.</li>
                                    <li>Більший AMOLED-екран.</li>
                                </ul>",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    AuthorEmail = "editor_gadget@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "maria_design@gmail.com", Content = "GPS в браслеті — давно пора!", CreatedAt = DateTime.Now.AddDays(-1) }
                    }
                },
                new Post
                {
                    Title = "Sony розкрила перші деталі PlayStation 6",
                    CategoryId = 3,
                    ShortDescription = "Консоль отримає 8K-ігри та повну інтеграцію з хмарним рендерингом.",
                    ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fimg.tamindir.com%2F2023%2F07%2F253503%2Fplaystation-6-2.jpg&f=1&nofb=1&ipt=14f1e413ec6c492a1f09d395683f716b06e538de81d0829cd3025819926c8d07",
                    Content = @"<p>Sony підтвердила, що PS6 вийде у 2028 році.</p>
                                <p>Особливості:</p>
                                <ul>
                                    <li>Гібридний рендеринг (локально + хмара).</li>
                                    <li>Підтримка 8K 60fps.</li>
                                    <li>Новий контролер DualSense Pro.</li>
                                </ul>",
                    CreatedAt = DateTime.Now.AddDays(-5),
                    AuthorEmail = "editor_gadget@technews.com"
                },
                new Post
                {
                    Title = "ASUS представила ROG Phone 9: 180 Гц та активне охолодження",
                    CategoryId = 3,
                    ShortDescription = "Найагресивніший ґеймерський смартфон року.",
                    ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fwww.notebookcheck.net%2Fuploads%2Ftx_nbc2%2FBild_Asus_ROG_Phone_9_Pro_Test_2024-9676.jpg&f=1&nofb=1&ipt=69ad7c906a9c8cf229cb74c9ec2991567e7d4aea42e5e6da4fc773066d904d89",
                    Content = @"<p>ROG Phone 9 отримав екран 180 Гц та зовнішній кулер AeroActive 9.</p>
                                <p>Він працює на Snapdragon 8 Gen 5 та має 24 ГБ RAM.</p>",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    AuthorEmail = "editor_gadget@technews.com"
                },

                // --- ШІ (Category 4) ---
                new Post
                {
                    Title = "Meta представила Llama 4: пришвидшена обробка та краще логічне мислення",
                    CategoryId = 1,
                    ShortDescription = "Модель демонструє на 30% менше помилок у reasoning-тестах.",
                    ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fwww.outrightcrm.com%2Fwp-content%2Fuploads%2F2025%2F04%2Fmeta-llama4-ai-model.jpg&f=1&nofb=1&ipt=016d1cd879c7b8524caa5367f36c8d7f14e6f467076695327d4cf9b1f5ab4ee4",
                    Content = @"<p>Llama 4 стала значним кроком уперед у порівнянні з Llama 3.</p>
                                <p>Покращено:</p>
                                <ul>
                                    <li>Математичні здібності.</li>
                                    <li>Структуроване письмo.</li>
                                    <li>Контекст до 1 млн токенів.</li>
                                </ul>",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    AuthorEmail = "editor_ai@technews.com"
                },
                new Post
                {
                    Title = "Sora від OpenAI: Кінець кінематографу?",
                    CategoryId = 4,
                    ShortDescription = "Нова модель створює відео реалістичної якості за текстовим описом.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/4d/OpenAI_Logo.svg/1024px-OpenAI_Logo.svg.png",
                    Content = @"<p>OpenAI знову шокувала світ. Їхня нова модель <strong>Sora</strong> може генерувати хвилинні відео у високій якості (1080p) просто за текстовим промптом.</p>
                                <p>Відео демонструють складні рухи камери, відображення в дзеркалах та фізику рідин.</p>
                                <p style='color: red;'>Експерти попереджають про нову еру фейків та дезінформації.</p>",
                    CreatedAt = DateTime.Now.AddDays(-3),
                    AuthorEmail = "editor_ai@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "maria_design@gmail.com", Content = "Як дизайнер кажу - це лякає. Скоро ми залишимось без роботи?", CreatedAt = DateTime.Now.AddHours(-10) },
                        new Comment { AuthorEmail = "pro_gamer@gmail.com", Content = "Тепер фільми можна робити вдома!", CreatedAt = DateTime.Now.AddHours(-5) }
                    }
                },
                new Post
                {
                    Title = "GitHub Copilot Workspace: ШІ пише проекти за вас",
                    CategoryId = 4,
                    ShortDescription = "GitHub анонсував нове середовище, де ШІ може спланувати та написати код цілого проекту.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c2/GitHub_Invertocat_Logo.svg/1200px-GitHub_Invertocat_Logo.svg.png",
                    Content = @"<p>Це більше не просто автодоповнення. <strong>Copilot Workspace</strong> розуміє контекст всієї вашої репозиторії.</p>
                                <ul>
                                    <li>Ви описуєте задачу (issue).</li>
                                    <li>ШІ пропонує план змін.</li>
                                    <li>ШІ пише код і запускає тести.</li>
                                </ul>
                                <p>Розробникам залишається роль архітекторів та рев'юерів коду.</p>",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    AuthorEmail = "editor_ai@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "alex_dev@gmail.com", Content = "Тестував бету - це магія, але за ним треба перевіряти.", CreatedAt = DateTime.Now.AddMinutes(-30) },
                        new Comment { AuthorEmail = "qa_ninja@gmail.com", Content = "Більше коду - більше багів. У мене буде більше роботи :)", CreatedAt = DateTime.Now.AddMinutes(-10) }
                    }
                },
                new Post
                {
                    Title = "Google Gemini 1.5 Pro: Конкурент GPT-4o?",
                    CategoryId = 4,
                    ShortDescription = "Google оновила свою флагманську модель. Тепер вона має контекстне вікно в 2 мільйони токенів.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/8a/Google_Gemini_logo.svg/2560px-Google_Gemini_logo.svg.png",
                    Content = @"<p>Величезне контекстне вікно дозволяє завантажувати цілі книги, кодові бази або довгі відео для аналізу.</p>
                                <p>У тестах на логіку та кодування Gemini 1.5 Pro показує результати, порівнянні з GPT-4o, а в деяких випадках і перевершує їх.</p>",
                    CreatedAt = DateTime.Now,
                    AuthorEmail = "editor_ai@technews.com",
                    Comments = new List<Comment>()
                },
                new Post
                {
                    Title = "Microsoft випускає Phi-4: компактну модель, що обганяє GPT-4o Mini",
                    CategoryId = 4,
                    ShortDescription = "Неймовірна ефективність при малому розмірі.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/44/Microsoft_logo.svg/800px-Microsoft_logo.svg.png",
                    Content = @"<p>Microsoft презентувала <strong>Phi-4</strong> — нове покоління компактних моделей для локального запуску.</p>
                                <p>Особливості:</p>
                                <ul>
                                    <li>Розмір 6–12B параметрів.</li>
                                    <li>Продуктивність на рівні GPT-4o Mini.</li>
                                    <li>Оптимізація для мобільних чипів Snapdragon X.</li>
                                </ul>",
                    CreatedAt = DateTime.Now.AddDays(-3),
                    AuthorEmail = "editor_ai@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "alex_dev@gmail.com", Content = "Мрію запускати це на Raspberry Pi 😅", CreatedAt = DateTime.Now.AddDays(-2).AddHours(7) }
                    }
                },
                new Post
                {
                    Title = "Runway Gen-4 дозволяє генерувати відео зі стилем обраного фільму",
                    CategoryId = 4,
                    ShortDescription = "Можна вибрати стиль Matrix, Avatar, Blade Runner та інші.",
                    ImageUrl = "https://external-content.duckduckgo.com/iu/?u=http%3A%2F%2Fiaboxtool.es%2Fwp-content%2Fuploads%2F2023%2F09%2Frunwayml-logo.png&f=1&nofb=1&ipt=3a1c11160b1c0bea895df45da5f70c3e8545acc3ce2783b667771d53f79ec5ca",
                    Content = @"<p>Компанія <strong>Runway</strong> представила модель Gen-4.</p>
                                <p>Вона дозволяє:</p>
                                <ul>
                                    <li>Генерувати відео у певному кінематографічному стилі.</li>
                                    <li>Імітувати роботу конкретних режисерів.</li>
                                    <li>Генерувати 4K-кадри.</li>
                                </ul>",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    AuthorEmail = "editor_ai@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "pro_gamer@gmail.com", Content = "Скоро будемо знімати фільми вдома за вечір.", CreatedAt = DateTime.Now.AddHours(-6) }
                    }
                },
                new Post
                {
                    Title = "Meta представила AI-аватарів, що говорять вашим голосом",
                    CategoryId = 4,
                    ShortDescription = "Генерація персонального дублера за 30 секунд аудіо.",
                    ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fblog.trendone.com%2Fjythulso%2F2023%2F01%2FMeta_Platforms_Inc._logo.svg_.png&f=1&nofb=1&ipt=55be3354ea8a7750048bb2511a9ea0c62555af7794fba9102cbed010faf211b7",
                    Content = @"<p>Новий сервіс Meta дозволяє створити ваш голосовий аватар для дзвінків та відео.</p>
                                 <p>Підтримує емоції, інтонацію і навіть стиль мови.</p>",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    AuthorEmail = "editor_ai@technews.com",
                    Comments = new List<Comment>
                    {
                        new Comment { AuthorEmail = "maria_design@gmail.com", Content = "Тепер точно можна знімати deepfake-дзвінки...", CreatedAt = DateTime.Now.AddHours(-2) }
                    }
                },
            };

            context.Posts.AddRange(posts);
            await context.SaveChangesAsync();
        }
    }
}
