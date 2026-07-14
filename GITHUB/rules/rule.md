## Reglas definidas para creacion de ramas al repositorio.

En este archivo se define que hacer con cada cambio que se realiza al proyecto.

1. Nombres para ramas de agregacion de archivos.
    - El nombre debe seguir el siguiente formato: debe empezar con "Add:".
    - Ejemplo: Add: Funcion para filtrar usuarios. ( en ingles siempre Add: Function to filter users).

2. Nombres para ramas de modificacion de archivos.
    - El nombre debe seguir el siguiente formato: debe empezar con "Modify:".
    - Ejemplo: Modify: Funcion para filtrar usuarios. ( en ingles siempre Modify: Function to filter users).

3. Nombres para ramas de eliminacion de archivos.
    - El nombre debe seguir el siguiente formato: debe empezar con "Delete:".
    - Ejemplo: Delete: Funcion para filtrar usuarios. ( en ingles siempre Delete: Function to filter users).

4. Nombres para ramas de correccion de errores.
    - El nombre debe seguir el siguiente formato: debe empezar con "Fix:".
    - Ejemplo: Fix: Funcion para filtrar usuarios. ( en ingles siempre Fix: Function to filter users).

5. Nombres para ramas de creacion de tests.
    - El nombre debe seguir el siguiente formato: debe empezar con "Test:".
    - Ejemplo: Test: Funcion para filtrar usuarios. ( en ingles siempre Test: Function to filter users).

6. Nombres para ramas de documentacion.
    - El nombre debe seguir el siguiente formato: debe empezar con "Docs:".
    - Ejemplo: Docs: Funcion para filtrar usuarios. ( en ingles siempre Docs: Function to filter users).

7. Nombres para ramas de refactorizacion.
    - El nombre debe seguir el siguiente formato: debe empezar con "Refactor:".
    - Ejemplo: Refactor: Funcion para filtrar usuarios. ( en ingles siempre Refactor: Function to filter users).

8. Nombres para ramas de merging.
    - El nombre debe seguir el siguiente formato: debe empezar con "Merge:".
    - Ejemplo: Merge: Funcion para filtrar usuarios. ( en ingles siempre Merge: Function to filter users).

 Commits: 
    - Los commits deben seguir el siguiente formato: deben empezar con "Add:", "Modify:", "Delete:", "Fix:", "Test:", "Docs:", "Refactor:", "Merge:".
    - El mensaje debe estar en ingles siempre.
    - Describiendo brevemente que se hizo.

## Las nuevas ramas de ajustes o agregaciones se deben sacar desde la rama DEVELOP.

    Luego de subir ese cambio a la nueva rama se debe hacer el merge a DEVELOP.

*git merge nombre de la rama* 
(Esta linea solo es un ejemplo de como se hace el merge, no es necesario escribirlo en la nueva rama)

## El merge siempre debe ser contra la rama DEVELOP

despues de develop de pasa a la rama master

## Nombres para subidas directa a la rama developement o master con cambios en distintos archivos.
    - El nombre debe seguir el siguiente formato: debe empezar con "Dev:".
    - El mensaje debe estar en ingles siempre.
    - Describiendo brevemente que se hizo.
    - Ejemplo: Dev: Ajustes varios en la base de datos. ( en ingles siempre Dev: Various adjustments in the database).