

Pontificia Universidad Cato´lica de Chile Escuela de Ingenier´ia
Departamento de Ciencia de la Computacio´n IIC2113 Disen˜o Detallado de Software

Entrega 1: Shin Megami Tensei
Francisco Ignacio Gazitu´a Requena Cristian Andr´es Hinostroza Espinoza
Introduccio´n
En esta entrega debes implementar el flujo principal del combate, lo que abarca el flujo completo del combate (aproximadamente desde la Secci´on 1 hasta la Secci´on 6 del enunciado) excluyendo cualquier habilidad, todas las afinidades menos Neutral y las acciones de invocar y pasar turno. Es decir, debes implementar:
La validaci´on de los equipos.
El flujo de rondas entre los jugadores.
Las acciones atacar, disparar y rendirse.
El setup del tablero
Leer las unidades desde archivos json.
C´alculos de dan˜o sin considerar afinidades.
Flujo completo del combate, considerando el consumo de turnos y el termino del combate.
Los test no simulan combates donde el rival tenga afinidades distintas a Neutral a los ataques del rival, por lo que solo debes implementar un caso del sistema de turnos.

Test cases
Para esta entrega debes completar los siguientes grupos de tests:  E1-BasicCombat.
 E1-InvalidTeams.  E1-Random
Cada test consiste en un archivo de texto que contiene el input y output esperado de tu programa. Todos los test cases se encuentran en el archivo data.zip. El siguiente es un extracto de uno de estos:

1
2
3
4
5
6
7
8
9
10
11
12

13
14
15
16
17
18
19
20
21
22
23
24
25
26
27
28
29
30
31
32
33
34
35
36
37
38
39
40
41
42
43
44
45
46
47
48
49
50
51
52
53

Tu programa debe generar el mismo output que aparece en el test para estar correcto. Notar que el test incluye el keyword “INPUT: ” en algunas l´ıneas (e.j., en la l´ınea 13). Esto indica que hay que pedir un input al usuario (en vez de escribir algo) y el input ingresado por el usuario ser´a el nu´mero que aparece luego de “INPUT: ”. Por ejemplo, si “INPUT: 2” entonces el nu´mero ingresado por el usuario ser´a 2.
Los tests de esta entrega comprueban que se cumplan dos escenarios.
El primer escenario consiste en verificar que el equipo elegido por cada jugador sea v´alido. Recordar que un equipo es inv´alido cu´ando:
 No tiene samurai.
 Tiene m´as de un samurai.
 Tiene m´as de 8 unidades (incluyendo al samurai).  Tiene alguna unidad repetida.
 Un samurai tiene m´as de 8 habilidades.
 Un samurai tiene alguna habilidad repetida.

Si un equipo es inv´alido, se muestra un mensaje indicando que la selecci´on es inv´alida y termina el programa:

1
2
3
4
5
6
7

El segundo escenario es que ambos equipos ingresados sean v´alidos. Si esto ocurre, el juego comenzar´a, mo- mento en el que los jugadores decidir´an qu´e acciones tomar´an sus unidades. El juego continuar´a normalmente hasta que uno de los jugadores se rinda o quede sin unidades vivas en su tablero. Este caso ser´a explicado con m´as detalle en las siguientes secciones.

Formato equipos
Lo primero que debe hacer tu programa es pedirle al usuario que seleccione un equipo. Cada grupo de test cases tiene un conjunto de equipos posibles disen˜ados para verificar el correcto funcionamiento de tu programa. Los equipos tambi´en se encuentran en data.zip. Por ejemplo, los equipos usados en los test cases E1-BasicCombat est´an en data/E1-BasicCombat/.
Para saber qu´e equipos se pueden utilizar, la clase Game.cs (que es la entrada a tu programa) recibe en su constructor el par´ametro teamsFolder. Este par´ametro contiene la ruta a la carpeta con todos los equipos disponibles segu´n el test case que se est´e ejecutando. Esta carpeta tendr´a un archivo .txt por cada equipo posible para cada jugador. Los equipos deben ser mostrados al usuario y luego se le debe pedir como input que elija alguno de ellos. En el siguiente ejemplo, hay 6 equipos posibles y el usuario elige el equipo 0:

1
2
3
4
5
6
7
8

Luego de elegido el equipo hay que leer el archivo y ver si el equipo es v´alido. Los archivos de equipos tienen el siguiente formato. La primera l´ınea indica que comienza la secci´on del primer jugador, seguida inmediatamente de los nombres de sus unidades. Luego de las unidades del primer jugador habr´a una l´ınea indicado el inicio de la secci´on del segundo jugador, seguida por el nombre de las unidades de su equipo. Por ejemplo, este es un posible equipo:

1
2
3
4
5
6
7
8
9
10
11
12
13
14
15

Los samurai siempre vienen precedidos por el indicador [Samurai], que permite diferenciarlo del resto de unidades. En caso que un samurai tenga habilidades, estas se encuentran en la misma l´ınea del nombre del samurai entre par´entesis. Si un samurai tiene m´as de una habilidad, estas se encontrar´an dentro del par´entesis delimitadas por una coma sin espacio entre ellas. En el ejemplo anterior, Joker tiene las habilidades Holy Wrath y Needle Shot, mientras que Nanashi no tiene habilidades.

Formato Habilidades
La informaci´on de las habilidades del juego est´a en el archivo: skills.json. Por cada habilidad se indica su nombre, tipo, costo, skill power, objetivo, hits y efectos. Sin embargo, en este entrega solo usaremos el nombre de la habilidad para verificar si un equipo es v´alido. La l´ogica para implementar cada habilidad ser´a agregada en las siguientes entregas.

1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22

Formato Unidades
La informaci´on de las unidades del juego se encuentra en el archivo json. Los atributos de cada unidad te permitir´an computar cu´anto dan˜o realizan al rival durante sus batallas y el orden en que actuar´an (entre otras cosas).
Los datos correspondientes a los Samurai se encuentra en el archivo samurai.json, el cual cuenta con el siguiente formato:


| 1 [ |  |  |
| --- | --- | --- |
| 2 | {...} , |  |
| 3 | { |  |
| 4 |  | " name ": " Demi - Fiend ", |
| 5 |  | " stats ": { |
| 6 |  | " HP ":  1227 , |
| 7 |  | " MP ": 834 , |
| 8 |  | " Str ":  273 , |
| 9 |  | " Skl ":  125 , |
| 10 |  | " Mag ":  300 , |
| 11 |  | " Spd ":  161 , |
| 12 |  | " Lck ":  151 |
| 13 |  | }, |
| 14 |  | " affinity ": { |
| 15 |  | " Phys ": " Nu", |
| 16 |  | " Gun ": "-", |
| 17 |  | " Fire ": " Nu", |
| 18 |  | " Ice ":  " Nu", |
| 19 |  | " Elec ": " Nu", |
| 20 |  | " Force ": " Nu", |
| 21 |  | " Light ": " Nu", |
| 22 |  | " Dark ": " Nu" |
| 23 |  | } |
| 24 | }, |  |
| 25 | { |  |
| 26 |  | " name ": " Tadano ", |
| 27 |  | " stats ": { |
| 28 |  | " HP ": 388 , |
| 29 |  | " MP ": 193 , |
| 30 |  | " Str ": 27 , |
| 31 |  | " Skl ": 23 , |
| 32 |  | " Mag ": 28 , |
| 33 |  | " Spd ": 30 , |
| 34 |  | " Lck ":  43 |
| 35 |  | }, |
| 36 |  | " affinity ": { |
| 37 |  | " Phys ": " Rs", |
| 38 |  | " Gun ": "-", |
| 39 |  | " Fire ": "-", |
| 40 |  | " Ice ": "-", |
| 41 |  | " Elec ": "-", |
| 42 |  | " Force ": "-", |
| 43 |  | " Light ": " Nu", |
| 44 |  | " Dark ": "-" |
| 45 |  | } |
| 46 | }, |  |
| 47 | {...} |  |
| 48 ] |  |  |


Por otro lado, los datos correspondientes a los monstruos se encuentra en el archivo monsters.json, el cual cuenta con el siguiente formato:


| 1 [ |  |  |
| --- | --- | --- |
| 2 | {...} , |  |
| 3 | { |  |
| 4 |  | " name ": " Night Stalker", |
| 5 |  | " stats ": { |
| 6 |  | " HP ": 231 , |
| 7 |  | " MP ": 103 , |
| 8 |  | " Str ": 29 , |
| 9 |  | " Skl ": 27 , |
| 10 |  | " Mag ": 27 , |
| 11 |  | " Spd ": 35 , |
| 12 |  | " Lck ":  29 |
| 13 |  | }, |
| 14 |  | " affinity ": { |
| 15 |  | " Phys ": "-", |
| 16 |  | " Gun ": "-", |
| 17 |  | " Fire ": "-", |
| 18 |  | " Ice ": "-", |
| 19 |  | " Elec ": "-", |
| 20 |  | " Force ": "-", |
| 21 |  | " Light ": " Wk", |
| 22 |  | " Dark ": "-" |
| 23 |  | }, |
| 24 |  | " skills ": [ |
| 25 |  | " Damascus Claw ", |
| 26 |  | " Dormina ", |
| 27 |  | " Life  Bonus" |
| 28 |  | ] |
| 29 | }, |  |
| 30 | { |  |
| 31 |  | " name ": " Tattooed Man ", |
| 32 |  | " stats ": { |
| 33 |  | " HP ": 290 , |
| 34 |  | " MP ": 66 , |
| 35 |  | " Str ": 35 , |
| 36 |  | " Skl ": 31 , |
| 37 |  | " Mag ": 24 , |
| 38 |  | " Spd ": 28 , |
| 39 |  | " Lck ":  23 |
| 40 |  | }, |
| 41 |  | " affinity ": { |
| 42 |  | " Phys ": "-", |
| 43 |  | " Gun ": "-", |
| 44 |  | " Fire ": "-", |
| 45 |  | " Ice ": "-", |
| 46 |  | " Elec ": "-", |
| 47 |  | " Force ": "-", |
| 48 |  | " Light ": " Nu", |
| 49 |  | " Dark ": " Wk" |
| 50 |  | }, |
| 51 |  | " skills ": [ |
| 52 |  | " Taunt", |
| 53 |  | " Fatal Sword ", |
| 54 |  | " Heat  Wave ", |
| 55 |  | " Counter" |
| 56 |  | ] |
| 57 | }, |  |
| 58 | {...} |  |
| 59 ] |  |  |


Como se puede ver, la unica diferencia entre estos archivos es que los monstruos tienen habilidades pre- definidas.

Output del juego
El programa siempre comienza mostrando los equipos que se pueden elegir dentro de la carpeta teamsFolder. Luego de ello, si el equipo es inv´alido, se notifica al usuario y termina el programa:

1
2
3
4
5
6
7

En el caso contrario, comenzar´a una batalla entre los equipos del Player 1 y del Player 2, donde siempre se comenzar´a con la ronda del Player 1.
Al iniciar la ronda del jugador, se mostrar´a un mensaje anunci´andolo, el que incluye el nombre del samurai del jugador. El siguiente ejemplo muestra esto para el jugador 1, quien ha escogido al samurai Flynn.

1
2

Luego de esto, se mostrar´a el estado actual del tablero. En este se mostrar´an siempre primero los puestos activos del jugador 1 seguido de los puestos activos del jugador 2. Solo para efectos del output, llamaremos a los puestos, de izquierda a derecha, como A, B, C y D respectivamente.
En orden, se mostrar´a el estado actual de cada puesto activo de izquierda a derecha, tengan estos unidades o no. En caso que tengan, se indicar´a su nombre, junto a su HP y MP; mientras que en caso de no tener, se mostrar´a el puesto vac´ıo.
El siguiente ejemplo ilustra una situaci´on donde ambos jugadores tienen el tablero lleno de unidades:

1
2
3
4
5
6
7
8
9
10
11
12
13

En este ejemplo, la linea A-Flynn HP:971/971 MP:527/527 significa que en la posici´on A se encuentra la unidad de nombre Flynn, la cual actualmente tiene 971 HP de un m´aximo de 971 y tiene 527 MP de un m´aximo de 527.
Inmediatamente seguido del estado del tablero, se mostrar´a cu´antos turnos tiene el jugador. En este ejemplo, el jugador tiene 4 Full Turns y ningu´n Blinking Turn:

1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16

Inmediatamente luego de esto, se mostrar´a el orden actual en el que actuar´a el equipo del jugador.

1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22

Luego de mostrar el estado del tablero, los turnos del jugador y el orden en que actu´an las unidades, se le pedir´a al usuario que seleccione una acci´on. Las posibles acciones que puede seleccionar depender´an de cu´al sea el tipo de unidad que est´a actuando. Si la unidad es un samurai, se desplegar´an las siguientes opciones:

1
2
3
4
5
6
7
8
9

Mientras que si la unidad es un monstruo, se desplegar´an las siguientes opciones:

1
2
3
4
5
6
7

Al seleccionar la opci´on Atacar, se mostrar´an las distintas unidades que el usuario tiene como objetivos. Las unidades se mostrar´an en el orden en el cual se encuentran en el tablero del oponente:

1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22
23
24
25
26
27
28
29
30
31
32
33
34
35
36
37
38
39

Vale la pena destacar, que a diferencia de las opciones de acciones para las unidades, las cuales est´an separadas del nu´mero correspondiente a cada acci´on con “: ” (dos puntos y un espacio), las selecciones para atacar est´an separadas por un gui´on. Adem´as, cada opci´on contiene el nombre de la unidad, su HP y su MP.
Luego de seleccionar la opci´on, se mostrar´a el resultado del ataque y el resultado sobre los turnos del jugador:

1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22
23
24
25
26
27
28
29
30
31
32
33
34
35
36
37
38
39
40
41
42
43
44
45
46

Por otro lado, la acci´on de Disparar se anuncia con el mismo formato que la acci´on anterior, pero se explicitar´a que la unidad us´o un disparo:

1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16

Como podemos ver, al usuario se le provee la opci´on Cancelar. Si el usuario selecciona esta opci´on, se deber´a desplegar nuevamente el menu´ con las acciones disponibles:

1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22
23
24
25
26

Otra de las opciones disponibles es Usar Habilidad. Si bien, en esta entrega no es necesario que implementen las habilidades, deben implementar la funcionalidad de mostrar las habilidades disponibles. Solo se deben mostrar las habilidades que la unidad efectivamente puede utilizar, es decir, aquellas cuyo costo sea menor o igual al MP actual de la unidad. Podr´ıa suceder que una unidad tenga solo habilidades que no puede utilizar, caso en que solo se desplegar´a la opci´on Cancelar.

Para efectos de esta entrega, el jugador siempre escoger´a la opci´on Cancelar luego de escoger utilizar una habilidad. El formato en que esto se realizar´a ser´a el siguiente:

1
2
3
4
5
6
7
8
9
10
11
12
13
14

Una vez terminado el turno de una unidad, si el jugador au´n tiene turnos disponibles, comenzar´a el turno de la siguiente unidad en el orden de acci´on. Al igual que en el caso anterior, se anunciar´a el estado actual del tablero, la cantidad de turnos disponibles, el nuevo orden de acciones y el input del usuario. Lo u´nico que no se mostrar´a ser´a el mensaje de inicio de ronda, ya que no ha cambiado la ronda del juego.

1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22
23
24
25
26
27

Existe la posibilidad de que un espacio del tablero se encuentre vac´ıo, sea porque un monstruo ha muerto o porque no se escogieron suficientes unidades al inicio del juego. En estos casos, los espacios se mostrar´an vac´ıos. Es importante sen˜alar que cuando un samurai muere, este no deja el tablero, por lo que sigue apareciendo en el anuncio del estado del tablero. Podemos ver el caso de espacios vac´ıos y samurai muertos en el siguiente ejemplo:

1
2
3
4
5
6
7
8
9
10
11

Una vez el jugador quede sin turnos, comenzar´a la ronda del otro jugador. Esta suceder´a de la misma manera y se anunciar´a con las mismas reglas que la ronda del otro jugador, siendo la u´nica diferencia que el mensaje de inicio de ronda mostrar´a los datos del otro jugador y el orden de las acciones corresponder´a al del otro equipo.
Si en cualquier punto del juego un equipo queda sin unidades vivas en el tablero, entonces el juego terminar´a. En este caso se anunciar´a al ganador y el programa terminar´a su ejecuci´on. El siguiente ejemplo muestra este caso:

1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21

Finalmente, el usuario tambi´en puede seleccionar la acci´on Rendirse. Si selecciona esta opci´on, se anunciar´a que el jugador se rindi´o, qui´en fue el ganador del juego y, finalmente, el programa terminar´a de ejecutarse. Sin embargo, no se mostrar´a el consumo de turnos:

1
2
3
4
5
6
7
8
9
10
11
12
13

C´alculo de dan˜o
Tal como indica el enunciado general del proyecto, los c´alculos de dan˜o pueden generar nu´meros decimales. Cuando ello ocurre, hay que truncar el nu´mero a su entero m´as bajo. Esto se puede realizar en C# utilizando la funci´on Math.Floor(...). Luego el resultado puede ser convertido a entero con Convert.ToInt32(...).

Input-Output
En tu proyecto NO debes usar Console.WriteLine(...) ni Console.ReadLine() para mostrar y pedir texto al usuario. Esto se debe a que nuestro c´odigo para comparar la salida de tu programa con los test cases ignora los mensajes mandados directamente a consola.
Para que el input-output de tu programa sea verificado por nuestros test cases debes usar el objeto view
que te entregamos en el constructor de Game.cs. Ese objeto tiene los siguientes m´etodos:
 ReadLine(): Solicita un string al usuario y retorna el string correspondiente.
 WriteLine(string message): Muestra message en consola.
El objeto view hace dos cosas. Por un lado, guarda los mensajes que se escriben mediante su m´etodo WriteLine(...). Esos mensajes son comparados con el test case para verificar si tu programa est´a correcto. Por otro lado, cuando se llama a ReadLine() autom´aticamente retorna el INPUT: indicado en el test case.
En resumen, todo input pedido y mensaje mostrado mediante Console es ignorado al momento de evaluar los test cases. Si quieres que un input o texto sea considerado debes utilizar los m´etodos de view.

Ru´brica
Para evaluar tu entrega usaremos 3 grupos de test cases: E1-Random, E1-InvalidTeams y E1-BasicCombat.
Esta entrega tiene puntaje por funcionalidad y por limpieza de c´odigo. Para calcular tu puntaje de funcio- nalidad se le asignar´a a cada grupo de test un puntaje m´aximo, el cual ser´a el limite superior del puntaje que obtendr´as en dicho grupo de tests; el puntaje que obtengas variar´a proporcionalmente a la cantidad de tests del grupo que pases. Los puntajes se distribuyen de la siguiente manera:
 [0.7 puntos] Porcentaje de test cases pasados en E1-InvalidTeams.  [3.0 puntos] Porcentaje de test cases pasados en E1-BasicCombat.  [2.3 puntos] Pasar todos los test cases en E1-Random.
Por ejemplo, digamos que tu entrega todos los test cases E1-InvalidTeams , todos los test cases E1-Random y el 80 % de los test cases E1-BasicCombat. Entonces tu puntaje de funcionalidad ser´a: 0,7+2,3+3,0·0,8 = 5,4.
Por otro lado, el puntaje por limpieza de c´odigo es en base a descuentos. Es decir, se parte con 6 puntos y se descuenta en base a las violaciones de los principios de los cap´ıtulos de Clean Code que presente tu c´odigo. Los descuentos m´aximos por cap´ıtulo son los siguientes:
 [ -2.0 puntos ] No sigue los principios del cap. 2 de clean code.
 [ -2.5 puntos ] No sigue los principios del cap. 3 de clean code.
Finalmente, tu nota final ser´a igual al promedio geom´etrico entre el puntaje por funcionalidad y el puntaje por limpieza de c´odigo (m´as el punto base), donde el promedio geom´etrico entre x e y es igual a √xy.
Por ejemplo, si tienes 3 puntos por funcionalidad y 5 puntos por limpieza de c´odigo entonces tu nota ser´a 3 · 5 + 1 = 4,9. Pero si tienes 6 puntos en funcionalidad y 1.5 en limpieza de c´odigo entonces tu nota ser´a 6 · 1,5 + 1 = 4,0.

Importante: No est´a permitido modificar los test cases ni el proyecto Shin-Megami-Tensei.Tests. Hacerlo puede conllevar una penalizaci´on que depender´a de la gravedad de la situaci´on
