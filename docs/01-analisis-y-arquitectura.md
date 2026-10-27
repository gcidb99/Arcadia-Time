# Arcadia Time: análisis y arquitectura

Versión 0.1 (Fase 0). Última actualización: 26/10/2026.

## 1. Resumen

Aplicación de escritorio para Windows que reúne minijuegos arcade con estética pixel art "cute". Un menú principal da acceso a los juegos, al registro de usuario por nombre y a la consulta de récords por usuario y juego. Se presenta como proyecto de fin de grado y se ampliará después de la entrega.

## 2. Objetivos

- Entregar un producto cerrado, jugable y documentado en el plazo del TFG.
- Partir de un código unificado: una sola arquitectura para todos los juegos, sin mezclar estilos de distintos tutoriales.
- Dejar el proyecto preparado para ampliarlo (más juegos, power-ups, base de datos remota, Android).
- Documentar el proceso a medida que se desarrolla, de modo que la memoria del TFG se construya durante el desarrollo.

## 3. Alcance

### Núcleo del TFG

- Menú principal con acceso a los tres juegos.
- Registro de nombre de usuario al iniciar el programa.
- Récords locales por usuario y juego, con una pantalla de consulta con pestañas por juego.
- Tres juegos completos: Cutie Breaker, Sweet Invaders y Antarctic Jump, con progresión de niveles o dificultad creciente.
- Compilación para Windows.

### Ampliaciones previstas (fuera del núcleo inicial)

- Power-ups y modificadores temporales en Cutie Breaker y Sweet Invaders. La arquitectura prevé los puntos de enganche desde el principio.
- Base de datos remota para usuarios y récords.
- Versión para Android.
- Más juegos arcade clásicos.

### Pendiente de confirmar con el tutor

Requisitos formales del TFG: extensión de la memoria, número mínimo de juegos, estructura exigida y fecha de entrega.

## 4. Tecnología y decisiones

| Aspecto | Decisión | Motivo |
|---|---|---|
| Motor | Unity 6.3 LTS (6000.3.27f1) | Versión LTS más reciente, con soporte anunciado hasta diciembre de 2027. |
| Lenguaje | C# | Es el único lenguaje de scripting que admite Unity. |
| Plantilla | Universal 2D | Proyecto 2D con URP y renderizador 2D preconfigurado. |
| IDE | Visual Studio Community 2026 | Integración directa con Unity. |
| Entrada | Input System de Unity | Permite añadir otros controles (por ejemplo táctil) sin reescribir los juegos. |
| Persistencia inicial | JSON local detrás de la interfaz `IRecordsRepository` | Suficiente para usuarios y récords locales, y sustituible después por una base de datos. |
| Control de versiones | Git con GitHub | Historial, copia de seguridad y documentación versionada. |

### Cambio de versión del motor

El primer intento del proyecto se hizo con Unity 2021.3.21f1. Se descarta por estar fuera de soporte y por ser anterior a las revisiones que corrigen la vulnerabilidad CVE-2025-59489 (ejecución local de código en aplicaciones compiladas con versiones afectadas). Las revisiones actuales de Unity 6.3 incluyen la corrección.

### Base de datos

Un juego de escritorio no debe conectarse directamente a MySQL, porque las credenciales quedarían dentro del ejecutable. Si se añade una base de datos remota en la ampliación, irá detrás de una API intermedia. Hasta entonces, la interfaz `IRecordsRepository` aísla el almacenamiento del resto del código.

## 5. Juegos

### Cutie Breaker (Brick Breaker)

El jugador controla una barra y una bola rebota rompiendo bloques. Cada bloque roto da puntos. Se pierde una vida si la bola cae por la parte inferior. El nivel termina al romper todos los bloques; la partida, al perder todas las vidas. Por niveles aparecen bloques que requieren más rebotes y dan más puntos.

### Sweet Invaders (Space Invaders)

El jugador mueve la nave inferior, que dispara automáticamente. Los enemigos bajan desde la parte superior y disparan al azar. Cada enemigo eliminado da puntos. Un impacto quita una vida y la partida termina al perder las tres. Por niveles aparecen enemigos más duros y que dan más puntos.

### Antarctic Jump (juego del dinosaurio de Chrome)

Un pingüino avanza hacia la derecha y salta sobre focas y pájaros. Cuanto más tiempo aguanta, más puntos obtiene y más rápido va el juego.

## 6. Arquitectura

### 6.1 Escenas

`Menu`, `Records`, `CutieBreaker`, `SweetInvaders`, `AntarcticJump`. Cada juego es una escena independiente.

### 6.2 Núcleo compartido (`ArcadiaTime.Core`)

- **ProfileService:** perfil de usuario activo y lista de perfiles.
- **RecordsService:** guarda y consulta la mejor puntuación por usuario y juego a través de `IRecordsRepository`.
- **SceneLoader:** carga de escenas y vuelta al menú.
- **GameServices:** punto de acceso estático a los servicios. Se inicializa solo antes de cargar la primera escena, de modo que cualquier escena de juego se puede probar directamente con Play.
- **AudioManager:** música y efectos.
- **MiniGame (clase base):** estados comunes (Listo, Jugando, Pausa, GameOver). Al terminar, cada juego envía su puntuación final al `RecordsService` de la misma manera. Se implementa en la Fase 2, junto al primer juego, para ajustarla a un caso real.

### 6.3 Sistemas reutilizables (`ArcadiaTime.Shared`)

- Vidas y puntuación.
- Progresión de niveles y dificultad, configurada con ScriptableObjects.
- Power-ups como ScriptableObjects con un efecto temporal (ampliación).
- Object pooling para proyectiles.

### 6.4 Datos con ScriptableObjects

- `GameDefinition`: nombre, imagen y escena de cada juego. El menú se construye a partir de la lista de definiciones, de modo que añadir un juego es crear un asset.
- Configuración de niveles y dificultad por juego.

### 6.5 Estructura de carpetas

```
Assets/
  _Project/
    Art/            (sprites, fondos, UI, fuentes)
    Audio/          (música y efectos)
    Prefabs/
    Scenes/
    ScriptableObjects/
    Scripts/
      Core/
      Shared/
      UI/
      Games/
        CutieBreaker/
        SweetInvaders/
        AntarcticJump/
```

## 7. Fases

| Fase | Contenido |
|---|---|
| 0 | Análisis, creación del proyecto, repositorio y estructura. |
| 1A | Núcleo: usuarios, récords, cargador de escenas y datos de los juegos. |
| 1B | Menú principal, pantalla de usuario y pantalla de récords. |
| 2 | Antarctic Jump (el más simple; valida la arquitectura de extremo a extremo). |
| 3 | Cutie Breaker. |
| 4 | Sweet Invaders. |
| 5 | Pulido, pruebas, compilación para Windows y memoria. |
| 6 | Ampliaciones (power-ups, base de datos, Android, más juegos). |

## 8. Riesgos

| Riesgo | Mitigación |
|---|---|
| Perder el plazo, como en el primer intento. | Núcleo cerrado y pequeño; ampliaciones solo después de la entrega o si sobra tiempo. |
| Inconsistencias entre juegos. | Clase base y sistemas compartidos desde la Fase 1. |
| Errores de una revisión concreta del motor. | Mantener una LTS y anotar en la bitácora cualquier problema conocido. |
| Pérdida de trabajo. | Commits frecuentes a GitHub. |

## 9. Preparación para Android

No es prioridad. Para no cerrar la puerta, se aplican estos hábitos desde el principio:

- Entrada a través del Input System, sin teclas escritas dentro de cada juego.
- Interfaz con Canvas Scaler y anclajes para adaptarse a distintas resoluciones.
- Guardado y rutas de archivo detrás de una sola capa.
