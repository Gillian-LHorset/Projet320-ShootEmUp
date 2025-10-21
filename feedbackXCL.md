## Feedback

### 10.9

- Livraison du 5.9:

  - Release: n'a pas été faite
  - Nommage des commits: Ne suit pas la convention. A mettre en oeuvre rapidement
  - Rapport: Je n'en vois aucun dans votre repo
  - User stories: Elles sont assez loin d'etre utilisables pour l'instant
  - Journal: Je ne vois aucune entrée pour le 3.9
  - Grille d'évaluation: Elle n'est pas dans le repo

- Attention !!! Sur la base de ce que je vois dans votre repo, votre projet est mal parti. Il faut réagir rapidement.
- J'ai fait un essai de version "scrolling", c'est faisable.

### 8.10

- Il y a un gros souci avec votre projet actuellement
- Le code ne fonctionne pas. Il fonctionne en fait moins bien maintenant que la semaine passée.
- Votre journal de travail est vide. Il ne donne aucune explication de pourquoi vous êtes bloqué. "beaucoup de bug de l'editeur" ne m'aide pas: quel éditeur ? quel bug ? pourquoi est-ce que ça vous bloque pendant 3 heures ?
- Il n'y a toujours rien de consistant en termes de rapport dans votre repo
- Je m'interroge: que faites-vous durant les périodes de projet ? Quelle que soit la réponse, il semble que ce ne soit pas adapté.

## 80%

Les valeurs possibles du résultat sont: LA (Largement Acquis), A (Acquis), I (Insuffisant), NA (non acquis)

| Critère                    | Résultat | Commentaire                                                                                                                                                                                                                                                                                                                                                                  |
| -------------------------- | -------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Avancement Obstacles       | A        | ce qui est une évaluation généreuse, étant donné que ce n'est pas votre code                                                                                                                                                                                                                                                                                                 |
| Avancement Joueur          | I        | la détection du contact avec le sol se fait par rapport à la position la plus à gauche de l'écran, pas la véritable position du vaisseau                                                                                                                                                                                                                                     |
| Avancement Tirs            | NA       | il n'y a rien concernant le tir. Au passage : je ne comprends pas pourquoi le premier test de acceptance de la US Tir et Interaction est cochée                                                                                                                                                                                                                              |
| Avancement ennemis         | NA       | il n'y a rien de fait à ce sujet                                                                                                                                                                                                                                                                                                                                             |
| Avancement score           | NA       | ici non plus                                                                                                                                                                                                                                                                                                                                                                 |
| Qualité Présentation       | A        | OK pour le peu de code que vous avez produit                                                                                                                                                                                                                                                                                                                                 |
| Qualité Commentaires       | I        | il n'y a pas de commentaire venant de vous dans le code                                                                                                                                                                                                                                                                                                                      |
| Qualité Conventions        | A        | un effort visible a été fait pour les suivre                                                                                                                                                                                                                                                                                                                                 |
| POO                        | I        | il n'y a toujours qu'une seule classe dans votre code.                                                                                                                                                                                                                                                                                                                       |
| Processus Journal          | I        | la version PDF de votre journal est illisible<br> le contenu n'est pas assez détaillé. 2h et 15 minutes pour gérer les collision avec le sol, c'est beaucoup de temps. Surtout quand on voit le peu de modifications apportées dans le code par le commit. Qu'est-ce qui a rendu cette tâche si longue ? Voilà le genre d'information que je veux retrouver dans un journal. |
| Processus Git              | I        | il vous faut faire preuve de plus de rigueur dans le nommage. <br>"Udate Scramble.csproj" ne suit pas la convention.<br> la gestion des collision avec le sol c'est du code, ce n'est pas une corvée (chore)                                                                                                                                                                 |
| Processus Livraison        | I        | il manque la grille d'évaluation dans la livraison                                                                                                                                                                                                                                                                                                                           |
| Expression User Stories    | I        | vous avez une maquette qui donne une bonne idée du gameplay. Mais vous ne l'utilisez pas dans l'énoncé de vos US.                                                                                                                                                                                                                                                            |
| Expression Rapport Forme   | I        | ajouter au minimum une page de garde                                                                                                                                                                                                                                                                                                                                         |
| Expression Rapport Contenu | I        | le rapport est pour ainsi dire vide.                                                                                                                                                                                                                                                                                                                                         |
| Ecologie (gitignore)       | A        |                                                                                                                                                                                                                                                                                                                                                                              |
| Comportement collectif     | A        |                                                                                                                                                                                                                                                                                                                                                                              |
| Comportement individuel    | I        | il vous faut améliorer fortement vos méthodes de travail. Il n'est pas concevable que vous fassiez si peu de progrès en hauteur de temps.                                                                                                                                                                                                                                    |

La situation de ce projet est critique. La quantité de choses à faire pour le mener à bien est grande.  
À la vitesse à laquelle vous avez progressé jusqu'ici, il est impossible d'arriver à un résultat satisfaisant.  
Je vous invite pourtant à ne pas baisser les bras, car les concepts nécessaires à la réalisation de ce projet sont absolument centraux dans les compétences d'un développeur.  

Je suis prêt à vous valider ce projet malgré tout, aux conditions suivantes :
- les déplacements du joueur sont corrects par rapport au sol qui bouge
- le vaisseau est capable de tirer des missiles devant lui
- des ennemis arrivent en sens inverse du vaisseau
- les ennemis sont détruits par les missiles du vaisseau
- votre rapport comporte un excellent diagramme de classes UML
- les commits montrent une démarche de développement structurée et cohérente
- vous êtes en mesure de m'expliquer de manière convaincante le fonctionnement de votre programme lors d'une code Review
