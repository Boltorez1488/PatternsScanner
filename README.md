![PatternsScanner](https://user-images.githubusercontent.com/89551246/132249559-234a812a-4df1-4d72-bb49-ae94866f56b2.jpg)

# Сканер шаблонов :ru:
**Windows EXE/DLL массивный AOB сканнер для реверсеров (x86/x64)**

Позволяет находить большое количество оффсетов в пару кликов для обновления софта.

Используемые шрифты:
- Inconsolate
- Roboto
- Showcard Gothic

Функционал:
- Поиск нескольких десятков, сотен адресов и более.
- Загрузка файла в память и поиск в мультипоточном режиме, за счёт чего достигается высокая скорость поиска.
- Выборка по выравненному байту, оффсету, порядковому номеру из найденных адресов, встраивание сразу множества адресов в одну ячейку.
- Запись результатов в результирующий версионный файл с макросами. Чтобы не копировать руками множество адресов.
- Поддерживает кучу вложенностей с блоками, комментируемыми слотами и шаблонами.
- Выводит информацию по ошибочным адресам, чтобы можно было быстро их скорректировать. Не ищет уже найденные адреса заново.

## Помощь
```
В example предоставлена примерная модель построения версионирования.
Так как шаблон универсальный, то рабочая директория должна быть именно example.

Рекомендуется использовать {exe} подстановку в OUT, HBUILD, 
дабы получить версионирование в виде кучки файлов - 1.0, 1.2, 2.0 и тд.

SCAN - Путь к сканируемому файлу.
OUT - Выходной текстовый дамп для отображения адресов.

MODBASE - HEX Смещение модуля в ОЗУ. У x32 исполняемых файлов по умолчанию 400000. Может быть равен нулю.

HPATTERN - Путь к универсальному макрос-шаблону, по которому будет строиться конечный файл макросов.
HBUILD - Конечный макрос-файл.

Можно использовать колесо мыши на разных полях, также действуют зажатия клавиш Alt || Ctrl || Shift.

Хоткеи:
  Главные:
    Ctrl+B - Создать блок
  Внутри блока:
    Ctrl+D - Удалить текущий
    Ctrl+A - Фокус на имени блока
    Ctrl+B - Создать вложенный блок
    Ctrl+F - Создать шаблон
    Ctrl+R - Создать комментирующий слот
    Shift+Up - Переместить выше
    Shift+Down - Переместить ниже
  Внутри шаблона/слота:
    Ctrl+D - Удалить текущий
    Shift+Up - Переместить выше
    Shift+Down - Переместить ниже
```

# Patterns scanner :us:
**Windows EXE/DLL massive AOB scanner for reversers (x86/x64)**

Allows you to find a large number of offsets in a couple of clicks to update the software.

Using fonts:
- Inconsolate
- Roboto
- Showcard Gothic

Features:
- Search for tens, hundreds of addresses or more.
- Loading a file into memory and searching in multi-threaded mode, due to which a high search speed is achieved.
- Sampling by aligned byte, offset, ordinal from found addresses, embedding multiple addresses in one field at once.
- Writing the results to the resulting versioned file with macros. In order not to copy many addresses by hands.
- Supports a bunch of nesting with blocks, commented slots and templates.
- Displays information on erroneous addresses so that you can quickly correct them. Doesn't search for already found addresses again.

## Help
```
In example an exemplary model of building versioning is provided. 
Since the template is universal, the working directory should be exactly example.

It is recommended to use {exe} substitution in OUT, HBUILD,
to get versioning as a bunch of files - 1.0, 1.2, 2.0 и тд.

SCAN - The path to the scanning file.
OUT - Output text dump for displaying addresses.

MODBASE - HEX Offset of the module in RAM. x32 executables have 400000 by default. Can be equal to zero.

HPATTERN - The path to the universal macro template, according to which the final macro file will be built.
HBUILD - Final macro file.

You can use the mouse wheel on different fields, pressing the Alt || Ctrl || Shift keys also works.

Hotkeys:
  Main:
    Ctrl+B - Create block
  In block:
    Ctrl+D - Delete this
    Ctrl+A - Focus to block name
    Ctrl+B - Create nested block
    Ctrl+F - Create pattern
    Ctrl+R - Create comment field
    Shift+Up - Move to up
    Shift+Down - Move to down
  In block/field:
    Ctrl+D - Delete this
    Shift+Up - Move to up
    Shift+Down - Move to down
```
