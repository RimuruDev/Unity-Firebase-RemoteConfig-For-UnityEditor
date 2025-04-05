# Unity Firebase Remote Config For Unity Editor

Этот небольшой набор скриптов позволяет в **Editor Mode** (без запуска Play Mode) принудительно обновлять значения из **Firebase Remote Config**, игнорируя кеш и не дожидаясь очередного перезапуска Unity или домейн‐релода.

---

## Предыстория

В старых (да и некоторых новых) версиях Firebase Remote Config для Unity есть проблема: при повторном запросе **вне** Play Mode SDK часто отдаёт устаревшие данные из кеша, пока не произойдёт перезагрузка скриптового домена (например, при запуске Play Mode или перезагрузке Unity). В результате любые изменения, опубликованные в Firebase Console, не сразу подтягиваются в редакторе.

**Решение** – «насильно» переинициализировать Firebase и очищать внутренние структуры, связанные с Remote Config, используя методы, недоступные напрямую (например, `FirebaseApp.DisposeAllApps()`), чтобы получить новую «сессию» Firebase и запросить актуальные данные.

---

## Как установить

1. Скопируйте три скрипта (`FirebaseAppHelper.cs`, `FirebaseRemoteConfigHelper.cs`, `RemoteConfigEditor.cs`) в свой Unity‐проект, желательно в папку **Editor**.
2. Убедитесь, что в проекте уже установлен и подключён **Firebase Remote Config SDK**.
3. Откройте в Unity меню **RimuruDev Tools → Firebase RemoteConfig Editor**.

---

## Как пользоваться

1. **Print RemoteConfig Cache**  
   Выводит в консоль текущее содержимое внутреннего словаря, где `FirebaseRemoteConfig` хранит свои экземпляры.

2. **Restart Firebase and Fetch Remote Config**
    - Очищает кеш Remote Config.
    - Через рефлексию вызывает `FirebaseApp.DisposeAllApps()`, «раз‐инициализируя» Firebase.
    - Повторно инициализирует Firebase, выставляет `MinimumFetchInterval` в ноль (чтобы отключить кеш).
    - Выполняет `FetchAsync` и `ActivateAsync`.
    - Выводит полученные актуальные данные в окно (и в консоль).

<img width="379" alt="image" src="https://github.com/user-attachments/assets/8fe247c4-e856-49a1-bcb8-686bf1c967fa" />

---

<img width="622" alt="image" src="https://github.com/user-attachments/assets/176b6636-22ea-43ba-ba48-6c91359f3de5" />

<img width="1146" alt="image" src="https://github.com/user-attachments/assets/e9bb707a-74c2-41ff-be8b-57c4291744eb" />

<img width="388" alt="image" src="https://github.com/user-attachments/assets/53852bb0-1d48-4894-ae84-4a62d8f09f81" />


Таким образом, вы можете многократно изменять параметры Remote Config в консоли, публиковать их, а затем нажимать «Restart Firebase and Fetch Remote Config» — и получать новые значения **сразу**, без запуска Play Mode и без перезапуска Unity.

---

## Важные детали

- Код основан на рефлексии и доступе к внутренним методам Firebase SDK, то есть **официально не поддерживается**. Однако для тестирования и отладки этого вполне достаточно.
- Тестировалось на:
    - **Firebase Remote Config** версии `12.2.1`
    - **Unity 2022.3.35f1**
- Успешно применялось в реальном проекте с большой пользовательской базой.
- В релизных сборках (на девайсах/платформах) кеш работает штатно; проблема возникает только при «горячем» тестировании в редакторе.

---

## Лицензия

Лицензируется под [MIT License](LICENSE).
