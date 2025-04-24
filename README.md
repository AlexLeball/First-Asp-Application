# Guide de Contribution/Utilisation – First-Asp-Application

## Prérequis

- Un compte **GitHub**  
- **Git** installé sur votre machine locale  
- Connaissances de base de **Git** et de la **ligne de commande**  
- Le **.NET SDK** installé (si nécessaire pour ce projet)

---

## Forker le Dépôt

1. Rendez-vous sur la page du dépôt original :  
   [https://github.com/AlexLeball/First-Asp-Application](https://github.com/AlexLeball/First-Asp-Application)

2. Cliquez sur le bouton **Fork** en haut à droite.  
   Cela créera une copie du dépôt dans votre propre compte GitHub.

---

## Cloner votre Dépôt Forké

1. Allez sur votre profil GitHub et ouvrez le dépôt forké.

2. Cliquez sur le bouton vert **Code** et copiez l’URL (HTTPS ou SSH).

3. Ouvrez votre terminal, puis exécutez la commande suivante pour cloner le dépôt sur votre machine locale :

   ```bash
   git clone https://github.com/VOTRE-UTILISATEUR/First-Asp-Application.git
   ```

**Accédez au dossier du projet :**  
Dans votre terminal, exécutez la commande suivante :  
```bash
cd First-Asp-Application
```

**Synchroniser avec le Dépôt Original**  
1. Ajoutez le dépôt original :  
   ```bash
   git remote add upstream https://github.com/AlexLeball/First-Asp-Application.git
   ```
2. Récupérez les dernières modifications depuis le dépôt original :  
   ```bash
   git pull upstream main
   ```

Ce guide a présenté les fonctionnalités de base de Git et GitHub, incluant le fork, le clonage, la récupération de changements et la mise à jour de votre copie locale.  
Vous êtes maintenant prêt(e) à visualiser et travailler sur le projet en local !
