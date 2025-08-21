

Pontificia Universidad Cato´lica de Chile Escuela de Ingenier´ia
Departamento de Ciencia de la Computacio´n IIC2113 Disen˜o Detallado de Software

Entrega 2: Shin Megami Tensei
Francisco Ignacio Gazitu´a Requena Cristian Andr´es Hinostroza Espinoza
Introduccio´n
En esta entrega debes agregar tres tipos de acciones a tu programa: Invocar, Pasar Turno y Usar Habilidad. En cuento a las habilidades, deber´as agregar habilidades ofensivas b´asicas con un solo objetivo, habilidades de Heal con un solo objetivo y una habilidad de tipo Special.
En esta entrega tambi´en deber´as implementar el sistema de afinidades del juego y las distintas reacciones que cada unidad puede tener sobre los distintos tipos de ataques y esto, a su vez, implicar´a que deber´as implementar el sistema de turnos del juego.

Test cases
Para esta entrega debes completar los siguientes grupos de tests:  E1-BasicCombat
 E1-InvalidTeams  E1-Random
 E2-PassTurnSummon
 E2-AffinityAndBasicSkills  E2-HealAndSabbatma
 E2-Random

Habilidades
Los monstruos vienen equipados con su propio set pre-definido de habilidades, mientras que los samurai pueden ser equipados con un m´aximo de 8. En esta entrega deber´as implementar habilidades ofensivas simples, habilidades de curaci´on simples y una habilidad especial.
A continuaci´on, listaremos las habilidades de esta entrega con el siguiente formato
[Type,Cost,Power,Target,Hits]. Name: Effect.
Las habilidades que tendr´an que implementar son:
 [Phys,6,90,Single,1] Lunge: Weak Phys attack. Taget: 1 enemy
 [Phys,9,150,Single,1] Oni-Kagura: Medium Phys attack. Target: 1 enemy
 [Phys,13,240,Single,1] Mortal Jihad: Heavy Phys attack. Target: 1 enemy

[Phys,5,100,Single,1]  Gram  Slice:  Weak  Phys  attack.  Target:  1  enemy [Phys,8,160,Single,1] Fatal  Sword: Medium Phys attack. Target: 1 enemy [Phys,12,250,Single,1] Berserker God: Heavy Phys attack. Target: 1 enemy [Phys,4,80,Single,1-3] Bouncing Claw: 1-3 weak Phys attacks. Target: 1 enemy [Phys,7,140,Single,1-3] Damascus Claw: 1-3 medium Phys attacks. Target: 1 enemy [Phys,11,230,Single,1-3] Nihil Claw: 1-3 heavy Phys attacks. Target: 1 enemy [Phys,15,220,Single,1-3] Axel Claw: 1-3 medium Phys attacks. Target: 1 enemy [Phys,8,120,Single,1] Iron Judgement: Medium Phys Attack. Target: 1 enemy [Phys,20,310,Single,1] Stigma Attack: Heavy physical damage to one foe. [Gun,4,75,Single,1]  Needle  Shot:  Weak  Gun  attack.  Target:  1  enemy [Gun,7,135,Single,1] Tathlum Shot: Medium Gun attack. Target: 1 enemy [Gun,11,225,Single,1]  Grand  Tack:  Heavy  Gun  attack.  Target:  1  enemy [Gun,30,350,Single,1]  Riot  Gun:  Severe  Gun  attack.  Target:  1  enemy [Fire,5,80,Single,1]   Agi:   Weak   Fire   attack.   Target:   1   enemy [Fire,8,140,Single,1]  Agilao:  Medium  Fire  attack.  Target:  1  enemy [Fire,14,210,Single,1]  Agidyne:  Heavy  Fire  attack.  Target:  1  enemy [Fire,22,300,Single,1]  Trisagion:  Severe  Fire  attack.  Target:  1  enemy [Ice,5,80,Single,1]   Bufu:   Weak   Ice   attack.   Target:   1   enemy [Ice,8,140,Single,1]  Bufula:  Medium  Ice  attack.  Target:  1  enemy [Ice,14,210,Single,1]  Bufudyne:  Heavy  Ice  attack.  Target:  1  enemy [Elec,5,80,Single,1]   Zio:   Weak   Elec   attack.   Target:   1   enemy [Elec,8,140,Single,1]  Zionga:  Medium  Elec  attack.  Target:  1  enemy [Elec,14,210,Single,1]  Ziodyne:  Heavy  Elec  attack.  Target:  1  enemy [Force,5,80,Single,1]   Zan:   Weak   Force   attack.   Target:   1   enemy [Force,8,140,Single,1]  Zanma:  Medium  Force  attack.  Target:  1  enemy [Force,14,210,Single,1]  Zandyne:  Heavy  Force  attack.  Target:  1  enemy [Force,22,300,Single,1] Deadly Wind: Severe Force attack. Target: 1 enemy [Heal,5,25,Ally,1] Dia: Heals HP. Target: 1 ally
[Heal,9,50,Ally,1]  Diarama:  Greatly  heals  HP.  Target:  1  ally [Heal,16,100,Ally,1]  Diarahan:  Fully  heals  HP.  Target:  1  ally [Heal,20,50,Ally,1] Recarm: Revive from KO with half HP. Target: 1 ally [Heal,40,100,Ally,1] Samarecarm: Revive from KO with full HP. Target: 1 ally
[Heal,45,100,Ally,1] Invitation: Summons another ally and revives the monster if dead. If the user is a monster, summons like a Samuari. Target: 1 ally

 [Special,20,0,Ally,1] Sabbatma: Summons another ally monster. If the user is a monster, summons like a Samuari.. Target: 1 ally

Output del juego
El formato de output del juego se mantiene igual al formato de la entrega anterior, a excepci´on de algunas adiciones para los nuevos elementos que deben an˜adir.

Pasar Turno
Cuando el usuario selecciona la acci´on Pasar Turno se mostrar´a su consumo de turnos, pero no se mostrar´a en consola ningu´n mensaje en particular:

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

Luego de esto, seguir´a el flujo del juego. Si el jugador se qued´o sin turnos, entonces empezar´a la ronda del rival, mientras que si au´n quedan turnos, iniciar´a la acci´on de la siguiente unidad en el orden de acciones.

Invocar
Esta acci´on funciona de forma distinta si es realizada por un samur´ai o un monstruo.
Si el samurai escoge la opci´on Invocar, se desplegar´a un menu´ donde se le pedir´a al usuario seleccionar un monstuo para invocar:

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

Los monstruos vivos de la reserva se mostrar´an en el orden en que aparecen en el archivo desde el cual se leen los equipos. Si no hay monstruos que se puedan invocar, entonces solo se desplegar´a la opci´on Cancelar.

Luego de que el samurai seleccione un monstruo, deber´a seleccionar la posici´on donde quiere colocar a la unidad invocada:

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

Los puestos se mostrar´an en orden de izquierda a derecha. Notar que si un puesto tiene a una unidad, se mostrar´a la informaci´on de la unidad seguida del puesto en el que se encuentra en par´entesis, mientras que si en el puesto no hay ninguna unidad, se mostrar´a Vac´ıo seguido del puesto en par´entesis. No se mostrar´a el Puesto 1 ya que en este siempre se encuentra el samurai, quien no se puede mover de ah´ı.
Luego de seleccionar el puesto donde se invocar´a a la unidad, se mostrar´a un mensaje indicando que la invocaci´on ha sido exitosa y se proceder´a a indicar cu´antos turnos se han consumido:

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

Por otro lado, si un monstruo utiliza la acci´on de Invocar, se desplegar´a el mismo menu´ de selecci´on de monstruo que se muestra cuando el samur´ai invoca; sin embargo, en vez de ser seguido por la selecci´on del puesto, ser´a seguido por el mensaje de ´exito seguido del consumo de turnos. Debido a que, cuando un monstruo invoca, el monstruo invocador es intercambiado por el monstruo invocado.

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

Afinidades
Los mensajes de dan˜o variar´an dependiendo de la afinidad al tipo de ataque, adem´as del dan˜o realizado (ver enunciado general).
Todos los casos de ataque en la entrega anterior eran ataques de afinidad Neutral:

1
2
3
4
5
6
7

Si el rival tiene la afinidad Weak sobre el ataque del rival, entonces se mostrar´a una l´ınea indicando esta situaci´on:

1
2
3
4
5
6
7
8

Si el rival tiene la afinidad Resist sobre el ataque del rival, entonces se mostrar´a una l´ınea indicando esta situaci´on:

1
2
3
4
5
6
7
8

Si el rival tiene la afinidad Null entonces el rival bloquear´a su ataque:

1
2
3
4
5
6
7

Si el rival tiene la afinidad Repel entonces el rival devolver´a el dan˜o al atacante. En este caso, se indicar´a que el rival devolvi´o el ataque y se mostrar´a el HP restante del atacante:

1
2
3
4
5
6
7

Si el rival tiene la afinidad Drain entonces el rival absorber´a el dan˜o del atacante. En este caso, se indicar´a que el dan˜o fue absorbido y se mostrar´a con cuanto HP queda la unidad:

1
2
3
4
5
6
7

Ataques Light y Dark
Dado que las habilidades Light y Dark matan al rival instant´aneamente, el mensaje que se mostrar´a ser´a ligeramente diferente para cada afinidad.
Si el rival tiene la afinidad Neutral sobre un ataque de tipo Light o Dark, el ataque podr´a acertar o fallar. Si el ataque es exitoso, se anunciar´a de la siguiente manera:

1
2
3
4
5
6
7

Por otro lado, si este ataque falla, se anunciar´a de la siguiente manera:

1
2
3
4
5
6
7


Si el rival tiene la afinidad Resist sobre un ataque de tipo Light o Dark, el ataque tambi´en podr´ıa fallar. Se anunciar´a de la siguiente manera estos dos casos:

1
2
3
4
5
6
7
8



1
2
3
4
5
6
7

Notar que no hay diferencias entre los anuncios de las afinidades Neutral y Resist si el ataque falla.
Si el rival tiene la afinidad Weak sobre un ataque de tipo Light o Dark, el ataque siempre acertar´a. Esto se anunciar´a de la siguiente manera:

1
2
3
4
5
6
7
8


Si el rival tiene la afinidad Null sobre un ataque de tipo Light o Dark, el ataque siempre ser´a bloqueado. Esto se anunciar´a de la siguiente manera:

1
2
3
4
5
6
7

Para esta entrega no hay casos en que los rivales puedan tener la afinidad Drain o Repel sobre ataques
Light o Dark.

Ataques elementales
Los ataques y las habilidades ofensivas simples pueden tener diversos elementos, entre los que se encuentran Phys, Gun, Fire, Ice, Elec y Force. Los ataques de cada uno de estos elementos se anuncian de la misma forma para cada tipo de afinidad que puede tener el rival. Sin embargo, el mensaje relacionado
a la acci´on cambiar´a.
Si se realiza un ataque Phys, sea porque se utiliz´o la acci´on atacar o una habilidad de tipo Phys, entonces se se anunciar´a utilizado la palabra ataca:

1
2
3
4


Si se realiza un ataque Gun, sea porque se utiliz´o la acci´on disparar o una habilidad tipo Gun, entonces se utilizar´a la palabra dispara:

1
2
3
4
5


Si se realiza un ataque Fire, entonces se utilizar´a la frase laza fuego:

1
2
3
4


Si se realiza un ataque Ice, entonces se utilizar´a la frase laza hielo:

1
2
3
4


Si se realiza un ataque Elec, entonces se utilizar´a la frase laza electricidad:

1
2
3
4


Si se realiza un ataque Force, entonces se utilizar´a la frase laza viento:

1
2
3
4


Las habilidades de muerte instant´anea pueden tener dos elementos, siendo estos Light y Dark. Al igual que para las habilidades ofensivas simples, el lenguaje que se utilizar´a al anunciar estos distintos elementos es un tanto distinto.

Si se realiza un ataque Light, entonces se utilizar´a la frase ataca con luz:

1
2
3
4
5


Si se realiza un ataque Dark, entonces se utilizar´a la frase ataca con oscuridad:

1
2
3
4

Habilidades ofensivas
Ahora veremos que pasa si una unidad utiliza la acci´on Usar Habilidad. Para esta entrega todas las habi- lidades tendr´an objetivo Single, por lo que el usuario deber´a escoger a su objetivo.
Luego que el usuario selecciona la acci´on y seleccione una habilidad ofensiva, se le preguntar´a a que rival quiere atacar. Los rivales se mostrar´an en el orden en que se encuentren en el tablero de izquierda a derecha

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

Luego, se anunciar´a el resultado de haber utilizado la habilidad, seguido del efecto de su uso en los turnos

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

Habilidades multi-hit
Si una habilidad realiza m´as de un golpe a la vez, entonces el anuncio se realizar´a de una forma un poco distinta. En general, se anunciar´a el efecto de cada golpe seguido del HP final de la unidad atacada.
Si una unidad realiza tres golpes y su rival tiene afinidad Neutral, entonces el resultado se anunciar´a de la siguiente manera:

1
2
3
4
5
6
7
8

Si una unidad realiza dos golpes y su rival tiene afinidad Weak, entonces el resultado se anunciar´a de la siguiente manera:

1
2
3
4
5
6
7
8

Si una unidad realiza dos golpes y su rival tiene afinidad Resist, entonces el resultado se anunciar´a de la siguiente manera:

1
2
3
4
5
6
7
8

Si una unidad realiza tres golpes y su rival tiene afinidad Null, entonces el resultado se anunciar´a de la siguiente manera:

1
2
3
4
5
6
7
8

Si una unidad realiza dos golpes y su rival tiene afinidad Repel, entonces el resultado se anunciar´a de la siguiente manera, recuerda que el HP que se muestra en este caso es el del atacante, no el del rival:

1
2
3
4
5
6

Si una unidad realiza tres golpes y su rival tiene afinidad Drain, entonces el resultado se anunciar´a de la siguiente manera:

1
2
3
4
5
6
7
8

Habilidades Heal
Existen dos tipos de habilidad Heal en esta entrega, aquellas que curan a una unidad y aquellas que reviven a una unidad. Para esta entrega, todas las habilidades Heal tendr´an objetivo Ally.
Para las habilidades que curan, pero no reviven, el flujo Input/Output ser´a el siguiente:
Cuando el usuario decida utilizar una habilidad de curaci´on, se desplegar´a un menu´ donde podr´a seleccionar a algu´n aliado en el tablero:

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

Notar que es posible seleccionar a la unidad que utiliza la habilidad y a unidades que tienen el 100 % de su HP.
Luego de seleccionar al aliado, se desplegar´a un mensaje anunciando el efeto de la habilidad, seguido de el consumo de turnos:

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

Por otro lado, si la unidad selecciona una habilidad que revive, se desplegar´a un menu´ donde se podr´a seleccionar a una unidad muerta:

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

Las unidades se mostrar´an en el orden en que aparecen en el archivo de selecci´on de equipos y solo se mostrar´an unidades muertas.

Luego de seleccionar al aliado, se desplegar´a un mensaje anunciando el efeto de la habilidad, seguido de el consumo de turnos:

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

Invitation
Esta invocar´a a un monstruo de la reserva como si estuviera siendo invocado por un samur´ai y, si este est´a muerto, lo revivir´a. Dado esto, seguir´a un flujo bastante similar a la invocaci´on de los samur´ais.
Cuando se seleccione la habilidad, se desplegar´a un menu´ para escoger a una unidad de la reserva, seguido por un menu´ para seleccionar el espacio donde se invocar´a:

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

Las unidades se mostrar´an en el orden en que aparecen en el archivo de selecci´on de equipos. Solo aparecer´an las unidades que est´an en la reserva, por lo que no se podr´a seleccionar al samur´ai (ya que, cuando el samur´ai muere, no se va a la reserva). Se mostrar´a tanto a las unidades que est´en vivas como a las que est´en muertas.
Si se selecciona a una unidad muerta, entonces se mostrar´a un mensaje indicando que la unidad fue invocada y revivida:

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

Por otro lado, si se selecciona a una unidad viva, se mostrar´a un mensaje que solo indica que esta fue invocada:

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

Sabbatma
La habilidad Sabbatma es una habilidad similar a Invitation, pero esta no puede revivir a los aliados. El flujo que seguir´a ser´a similar al de Invitation.
Cuando se seleccione la habilidad, se desplegar´a un menu´ para escoger a una unidad viva de la reserva, seguido por un menu´ para seleccionar el espacio donde se invocar´a:

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

Finalmente, se mostrar´a un mensaje que muestra el efecto de la habilidad:

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

C´alculos del combate
Tal como indica el enunciado general del proyecto, los c´alculos de dan˜o pueden generar nu´meros decimales. Cuando ello ocurre, hay que truncar el nu´mero a su entero m´as bajo. Esto se puede realizar en C# utilizando la funci´on Math.Floor(...). Luego el resultado puede ser convertido a entero con Convert.ToInt32(...).

Ru´brica
Esta entrega tiene puntaje por funcionalidad y por limpieza de c´odigo. El puntaje por funcionalidad es en base a descuentos. Es decir, se parte con 6 puntos y se descuenta en base al porcentaje de tests que no pasen de cada bater´ıa de tests. Los descuentos son:
 [ -3.0 puntos ] Porcentaje de test cases no pasados en E1-BasicCombat.  [ -0.7 puntos ] Porcentaje de test cases no pasados en E1-InvalidTeams.  [ -2.3 puntos ] Porcentaje de test cases no pasados en E1-Random.
 [ -0.5 puntos ] Porcentaje de test cases no pasados en E2-PassTurnSummon.
 [ -2.5 puntos ] Porcentaje de test cases no pasados en E2-AffinityAndBasicSkills.
 [ -2.0 puntos ] Porcentaje de test cases no pasados en E2-HealAndSabbatma.
 [ -0.8 puntos ] Porcentaje de test cases no pasados en E2-Random.
Al igual que el puntaje por funcionalidad, el puntaje por limpieza de c´odigo tambi´en es en base a descuentos. Los descuentos m´aximos por cap´ıtulo son los siguientes:

 [ -1.0 puntos ] No sigue los principios del cap. 2 de clean code.  [ -2.0 puntos ] No sigue los principios del cap. 3 de clean code.  [ -2.0 puntos ] No sigue los principios del cap. 6 de clean code.  [ -1.5 puntos ] No sigue los principios del cap. 10 de clean code.  [ -0.5 puntos ] No implementa MVC.
Finalmente, tu nota final ser´a igual al promedio geom´etrico entre el puntaje por funcionalidad y el puntaje por limpieza de c´odigo (m´as el punto base), donde el promedio geom´etrico entre x e y es igual a √xy. En
caso de que x o y sean negativos, tu nota ser´a un 1,0.
P√or ejemplo, si tienes 3 puntos por funcionalidad y 5 puntos por limpieza de c´odigo entonces tu nota ser´a 3 · 5 + 1√= 4,9. Pero si tienes 6 puntos en funcionalidad y solo 1 punto en limpieza de c´odigo entonces tu
nota ser´a	6 · 1 + 1 = 3,5.
Importante: No est´a permitido modificar los test cases ni el proyecto Shin-Megami-Tensei.Tests. Hacerlo puede conllevar una penalizaci´on que depender´a de la gravedad de la situaci´on

Bonus
Esta entrega contar´a con un bonus que puede permitir que tu puntaje de funcionalidad sea mayor a 6 puntos. Para obtener este puntaje debes implementar las siguientes habilidades:
 [Light,6,30,Single,1] Hama: Light instant kill. Target: 1 enemy
 [Light,10,55,Single,1] Hamaon: Light instant kill. Target: 1 enemy; High success
 [Dark,6,30,Single,1] Mudo: Dark instant kill. Target: 1 enemy
 [Dark,10,55,Single,1] Mudoon: Dark instant kill. Target: 1 enemy; High success
Existen 2 grupos de tests que prueban que estas habilidades funcionen. Los tests y su bonus correspondiente en el puntaje de funcionalidad son:
 [ +0.6 puntos ] Porcentaje de test cases pasados en E3-SingleTargetInstaKill.
 [ +0.5 puntos ] Porcentaje de test cases pasados en E3-RandomSingleTargetInstaKill.
Ten en cuenta que el c´odigo que escribas para implementar las habilidades del bonus ser´a considerado para determinar tu puntaje por limpieza de c´odigo y podr´ıa generar descuentos en esa categor´ıa.
