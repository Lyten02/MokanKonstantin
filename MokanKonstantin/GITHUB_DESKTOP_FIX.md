# Инструкция для GitHub Desktop

## Способ 1: Через меню Branch (Рекомендуется)

1. В GitHub Desktop перейдите в меню **Branch** → **Reset Current Branch**
2. Выберите **origin/dev** 
3. Нажмите **Reset Branch**
4. Подтвердите действие

## Способ 2: Через History

1. Перейдите во вкладку **History**
2. Найдите коммит **27374a6 delete comments**
3. Правый клик на этом коммите
4. Выберите **Reset Current Branch to This Commit**
5. Выберите **Hard Reset** (это удалит все изменения после этого коммита)

## Способ 3: Если не помогает - через терминал в GitHub Desktop

1. В GitHub Desktop: **Repository** → **Open in Terminal** (или Cmd+`)
2. Выполните команды:
```bash
git fetch origin
git reset --hard origin/dev
```
3. GitHub Desktop автоматически обновится

## ВАЖНО!
- НЕ нажимайте "Push origin" пока не выполните сброс
- После сброса GitHub Desktop должен показать "0 commits to push"
- Если есть несохранённые изменения, они будут потеряны при Hard Reset

## Проверка
После выполнения любого из способов:
- В History последний коммит должен быть "27374a6 delete comments"
- Не должно быть коммитов для отправки (Push origin)
- Статус должен быть "Up to date with origin/dev"