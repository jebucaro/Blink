# Blink
![Blink Logo](./logo.jpg)

Es una aplicaci�n de Windows que puede realizar tres actividades, estas actividades son accesibles al abrir el men� contextual sobre una carpeta:

+ Generar una estructura de carpetas de acuerdo a un archivo de configuraci�n
+ Listar el contenido de una carpeta en un archivo Excel. Dentro del archivo Excel se pueden listar los archivos en diferentes pesta�as, y dependiendo de la configuraci�n y la carpeta en la que estos actualmente se encuentren.
+ Eliminar los directorios hijos que se encuentren actualmente vac�os.


Por ejemplo, el siguiente archivo de configuraci�n:

```json
[
    {
        "name": "C�digo Fuente",
        "branch": [
            {
                "name": "Presentaci�n",
                "label": "Capa - Presentaci�n"
            },
            {
                "name": "Aplicaci�n",
                "label": "Capa - Aplicaci�n"
            },
            {
                "name": "Datos",
                "label": "Capa - Datos"
            }
        ]
    },
    {
        "name": "Documentaci�n",
        "browsable": false,
        "branch": [
            {
                "name": "Requerimiento"
            },
            {
                "name": "Minutas"
            },
            {
                "name": "Manuales"
            },
            {
                "name": "Documentaci�n T�cnica"
            }
        ]
    }
]
```
Crear� la siguente estructura de carpetas dento de la carpeta seleccionada, en este caso la carpeta ra�z *Ejemplo*:

![Ejemplo](./ejemplo.jpg)

El proyecto se encuentra desarrollado en C# y el instalador fue realizado con Inno Setup. Se colocaron los permisos m�nimos como para poder realizar la instalaci�n sin privilegios de usuario administrador, y �nicamente se instala para el usuario actual.

El proyecto esta sujeto a mejoras, as� que *sugerencias y modificaciones son bienvenidas!*

## Mejoras en funcionalidad que me gustar�a realizar
+ Agregar m�s pantallas en el instalador que demuestren la utilidad de la aplicaci�n
+ Agregar otra opci�n para modificar el archivo de configuraci�n en el men� contextual.
+ Detectar la configuraci�n del sistema operativo y dependiendo, ofrecer como m�nimo la veris�n en espa�ol (castellano) o la versi�n de ingl�s como m�nimo.
+ Agregar un mejor manejo de errores, considero que no deber�a tener tantos try catch al utilizar las clases de Blink_Lib, tal vez agregarle una clase que guarde el c�digo de error y mensaje, ya que de esa manera aprovecho la funcionalidad del estado de la operaci�n.

<hr>

## Constru�do con
+ C#
+ Newtonsoft JSON
+ EPPlus