# Bitácora de desarrollo

Diario del proyecto, con la entrada más reciente al final. Aquí se anotan decisiones, problemas encontrados y cómo se resolvieron, para reutilizarlos en la memoria del TFG y en la futura skill de creación de juegos.

## 26/10/2026 - Fase 0: preparación

**Hecho**
- Análisis del proyecto: alcance, juegos, arquitectura y fases (ver `01-analisis-y-arquitectura.md`).
- Revisión del material del primer intento en Unity 2021.3.21f1 (menú y tres juegos). Se decide rehacer el código desde cero con una arquitectura unificada y reutilizar solo los assets.
- Elección del motor: Unity 6.3 LTS, revisión 6000.3.27f1.
- Instalación del Editor con Visual Studio Community 2026 y soporte de compilación para Windows (Mono). Android se añadirá más adelante.
- Creación del proyecto con la plantilla Universal 2D, sin AI Assistant, sin Unity CLI y sin control de versiones desde el Hub.

**Decisiones**
- Lenguaje C#, el único que admite Unity.
- Tres juegos: Cutie Breaker, Sweet Invaders y Antarctic Jump. Flappy Bird queda descartado.
- Un único botón de Records con pestañas por juego.
- Persistencia inicial en JSON local detrás de `IRecordsRepository`; la base de datos remota queda como ampliación.
- Git y GitHub conectados a mano para controlar el `.gitignore`.

**Cierre de la fase**
- Proyecto conectado a GitHub y estructura de carpetas `Assets/_Project` creada.

**Pendiente**
- Confirmar con el tutor los requisitos formales del TFG.

**Notas de versión**
- La 6000.3.25f1 tenía un problema conocido de pantalla negra con Bloom y el renderizador 2D. No se ha comprobado si sigue presente en la 6000.3.27f1.

## 26/10/2026 - Fase 1A: núcleo de usuarios y récords

**Hecho**
- Scripts del núcleo en `Assets/_Project/Scripts/Core` (namespace `ArcadiaTime.Core`):
  - Datos de guardado: `SaveData`, `ProfileData`, `ScoreEntry`.
  - Almacenamiento: interfaz `IRecordsRepository` e implementación `JsonRecordsRepository`.
  - Servicios: `ProfileService` (usuarios), `RecordsService` (récords del usuario activo), `SceneLoader` y `SceneNames`.
  - Datos del menú: `GameDefinition` y `GameCatalog` (ScriptableObjects).
  - `GameServices`: punto de acceso estático a los servicios.
  - `CoreSmokeTest`: script temporal de comprobación.

**Decisiones**
- Los servicios son clases C# normales, sin objetos persistentes en escena. `GameServices` los crea con `RuntimeInitializeOnLoadMethod` antes de cargar la primera escena. Así cualquier escena de juego se puede probar con Play sin pasar por el menú.
- El archivo de guardado es `arcadia-time-save.json` en `Application.persistentDataPath`. Se escribe en un archivo temporal y luego se reemplaza, para no corromper el guardado si el programa se cierra a mitad de escritura. Si el archivo está dañado, se hace una copia con sufijo `.corrupto-` y se empieza uno nuevo.
- Nombre de usuario: de 2 a 12 caracteres, con letras, números, espacios, guiones y guiones bajos. Los nombres no distinguen mayúsculas de minúsculas.
- Una puntuación solo cuenta como récord si es mayor que 0 y supera la anterior.
- En el Editor, si se guarda una puntuación sin usuario activo, se usa un usuario llamado `Editor`. En un build no ocurre.
- `IRecordsRepository` es síncrona. Si en la ampliación se pasa a una base de datos remota, habrá que hacerla asíncrona y adaptar los servicios.

**Pendiente**
- Comprobar el núcleo con `CoreSmokeTest` y borrar después el script de prueba y los datos de prueba.
- Fase 1B: escena de menú, pantalla de usuario y pantalla de récords con pestañas.
