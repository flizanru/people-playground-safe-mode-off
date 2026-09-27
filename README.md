# People Playground — установка фикса модов

Фикс состоит из двух файлов, которые нужно положить в папку игры:

```text
People Playground_Data\Managed\Assembly-CSharp.dll
People Playground_Data\Managed\PpgModRuntimeBridge.dll
```

## Куда копировать

1. Закройте People Playground и Steam.
2. Откройте папку установленной игры Steam:

   ```text
   ...\Steam\steamapps\common\People Playground\
   ```

3. Из архива фикса скопируйте папку `People Playground_Data` в папку игры с заменой файлов.
4. Итоговый путь к файлам должен быть таким:

   ```text
   ...\People Playground\People Playground_Data\Managed\Assembly-CSharp.dll
   ...\People Playground\People Playground_Data\Managed\PpgModRuntimeBridge.dll
   ```

5. Запустите игру. В главном меню сверху появится надпись `Fix by flizan.com`. Значит вы всё сделали правильно

## Важно

- `Assembly-CSharp.dll` и `PpgModRuntimeBridge.dll` устанавливаются только вместе.
- При проверке целостности файлов Steam оригинальный `Assembly-CSharp.dll` будет восстановлен, а `PpgModRuntimeBridge.dll` может потребоваться удалить вручную.

## Как удалить фикс

1. Закройте игру.
2. В Steam выберите: **People Playground → Свойства → Установленные файлы → Проверить целостность файлов игры**.
3. Удалите файл:

   ```text
   ...\People Playground\People Playground_Data\Managed\PpgModRuntimeBridge.dll
   ```

Steam вернёт исходный `Assembly-CSharp.dll`.
