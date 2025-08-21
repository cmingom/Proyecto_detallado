# Reglas de Clean Code: Capítulos 6 y 10

## Capítulo 6: Objetos y estructuras de datos

### 1. Ocultar la implementación
- Aplicar el principio de **encapsulamiento**.
- Las clases deben ocultar los detalles internos de su estructura y exponer solo operaciones.

### 2. Accesores no equivalen a estructuras
- Si bien los getters y setters son comunes, exponerlos rompe la encapsulación.
- Usar métodos que representen comportamientos de negocio en lugar de exponer datos.

### 3. Los objetos exponen comportamiento, las estructuras exponen datos
- Objetos: tienen métodos que operan sobre su estado interno y lo encapsulan.
- Estructuras: permiten el acceso directo a los datos sin lógica asociada.

### 4. Elegir objetos cuando se desea ocultar la implementación
- Útiles para tareas donde el comportamiento es más importante que la estructura.

### 5. Elegir estructuras cuando se desea manipular datos libremente
- Útiles en capas de control, persistencia o transmisión de datos.

### 6. Ley de Demeter (principio de menor conocimiento)
- Un objeto no debe conocer la estructura interna de otros objetos.
- Mala práctica: `a.getB().getC().doSomething()`.

### 7. Modelar relaciones y responsabilidades con claridad
- No forzar un objeto a actuar como una estructura y viceversa.
- Separar objetos (comportamiento) y DTOs (estructura).

---

## Capítulo 10: Clases

### 1. Organización de clases
- Las clases deben ser **cortas**.
- Una clase debe tener una **única responsabilidad** (SRP: Single Responsibility Principle).

### 2. Número de responsabilidades
- Una clase debe tener solo **una razón para cambiar**.
- Si cambia por múltiples razones, debe dividirse.

### 3. Tamaño y cohesión
- Las clases largas suelen tener múltiples responsabilidades ocultas.
- Altamente cohesiva: todos los métodos están relacionados directamente con el propósito de la clase.

### 4. Ordenar por alto nivel primero
- Las clases deben leerse de **alto a bajo nivel**, igual que las funciones.
- Lo público primero: luego métodos privados de ayuda.

### 5. Agrupar variables y funciones relacionadas
- Variables privadas deben estar junto a las funciones que las usan.
- Ayuda a mejorar la cohesión visual.

### 6. Evitar clases con muchos métodos públicos
- Muchas funciones públicas implican demasiadas responsabilidades o falta de abstracción.

### 7. Clases deben ser abiertas a extensión pero cerradas a modificación
- Aplicar el principio OCP: Open/Closed Principle.
- Usar herencia o composición según corresponda.

### 8. Mantener código orientado a objetos
- Evitar mezclar funciones estáticas utilitarias con lógica OO principal.
- Si la lógica no depende de estado, considerar refactorizar hacia clase especializada o helper aislado.

