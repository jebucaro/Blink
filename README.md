# Blink
![Blink Logo](./logo.jpg)

Blink es una aplicaci�n de Windows que puede realizar por el momento tres actividades principales, estas actividades son accesibles al abrir el men� contextual sobre una carpeta:

+ Generar una estructura de carpetas de acuerdo a lo definido en un archivo de configuraci�n
+ Listar el contenido (archivos) de una carpeta en un archivo Excel. Dentro del archivo Excel se pueden listar los archivos en diferentes hojas, dependiendo de etiqueta asociada a la carpeta en el archivo de configuraci�n.
+ Eliminar los directorios hijos que se encuentren actualmente vac�os.

Se agreg� una opci�n para abrir el archivo de configuraci�n desde el men� contextual.

El siguiente ejemplo de archivo de configuraci�n:

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
Crear� la siguente estructura de carpetas dento de la carpeta seleccionada como carpeta de trabajo, en este caso la carpeta ra�z *Ejemplo*:

![Ejemplo](./ejemplo.jpg)

El proyecto se encuentra desarrollado en C# y el instalador fue realizado con Inno Setup. Se colocaron los permisos m�nimos como para poder realizar la instalaci�n sin privilegios de usuario administrador, y �nicamente se instala para el usuario actual. El directorio de instalaci�n es __%appdata%\Blink__.

## Mejoras en funcionalidad que me gustar�a realizar
El proyecto esta sujeto a muchas mejoras, as� que *sugerencias y modificaciones son bienvenidas!*

Estas son algunas de las mejoras que me gustar�a realizar.

+ ~~Agregar otra opci�n para modificar el archivo de configuraci�n en el men� contextual.~~
+ Agregar m�s pantallas en el instalador que demuestren la utilidad de la aplicaci�n
+ Detectar la configuraci�n del sistema operativo y dependiendo, ofrecer como m�nimo la versi�n en espa�ol (castellano) o la versi�n de ingl�s como m�nimo.

<hr>

## Constru�do con
+ C#
+ Newtonsoft JSON
+ EPPlus
+ Inno Setup

<hr>

## Iconos utilizados
+ [Recepkutuk](https://www.iconfinder.com/recepkutuk)
+ [Igorverizub](https://www.iconfinder.com/igorverizub)
+ [Glyphlab](https://www.iconfinder.com/glyphlab)
