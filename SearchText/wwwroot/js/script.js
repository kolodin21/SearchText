$(document).ready(function () {
    let timer;

    // Обработка ввода в поле поиска
    $('#searchBox').on('input', function () {
        const query = $(this).val().trim();

        clearTimeout(timer);

        if (query.length > 2) {
            timer = setTimeout(() => {
                fetch(`/api/notes/search?search=${query}`)
                    .then(response => {
                        if (!response.ok) {
                            throw new Error('Ошибка поиска');
                        }
                        return response.json();
                    })
                    .then(data => {
                        const suggestionsBox = $('#suggestionsBox');
                        suggestionsBox.empty();

                        if (data.length === 0) {
                            suggestionsBox.append('<div>Ничего не найдено</div>');
                        } else {
                            data.forEach(item => {
                                const suggestion = $(`<div>${item}</div>`);
                                suggestion.on('click', function () {
                                    $('#searchBox').val($(this).text());
                                    suggestionsBox.empty();
                                });
                                suggestionsBox.append(suggestion);
                            });
                        }
                    })
                    .catch(error => console.error('Ошибка при поиске:', error));
            }, 300); // Задержка 300 мс
        } else {
            $('#suggestionsBox').empty();
        }
    });

    // Обработка кнопки добавления заметки
    $('#addNoteButton').on('click', function () {
        const noteTitle = $('#noteInput').val().trim();

        if (noteTitle === "") {
            alert("Введите заголовок заметки!");
            return;
        }

        fetch('/api/notes/add', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ title: noteTitle })
        })
            .then(response => {
                if (response.ok) {
                    alert('Заметка успешно добавлена!');
                    $('#noteInput').val('');
                } else {
                    alert('Ошибка при добавлении заметки');
                }
            })
            .catch(error => console.error('Ошибка при добавлении заметки:', error));
    });

    $(document).ready(function () {
        console.log('JavaScript подключен и работает');

        $('#searchBox').on('input', function () {
            console.log('Событие input сработало');
        });

        $('#addNoteButton').on('click', function () {
            console.log('Событие click сработало');
        });
    });
});