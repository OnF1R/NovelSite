// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Layout

function searchFormHandler() {
    //var form = document.getElementById('searchForm');

    //var input = document.querySelector('input[name="visual-novel-search"]').value;

    //form.addEventListener("submit", event => {
    //    event.preventDefault();
    //    var { tags, genres, languages, platforms, spoilerLevel, readingTime, sort, search } = getFilters();

    //    const filters = {
    //        tags: tags,
    //        genres: genres,
    //        //authors: urlParams.get('authors') || '',
    //        //translators: urlParams.get('translators') || '',
    //        languages: languages,
    //        platforms: platforms,
    //        spoilerLevel: spoilerLevel,
    //        readingTime: readingTime,
    //        sort: sort,
    //        search: input
    //    };

    //    window.location.href = `results.html?query=${encodeURIComponent(query)}`;

    //    changePage(1, filters, false)
    //});
}

//End Layout

//Novel Page
//Ошибка 431 из-за размера Cookie при отправке запроса
//function trackVisitor(visualNovelId) {
//    var domain = window.location.origin;

//    fetch(`${domain}/Visitor/IncrementPageViewsCount?visualNovelId=${visualNovelId}`, {
//        method: 'PUT',
//        credentials: 'include' // Важно для отправки cookie
//    })
//        .then(response => {
//            if (!response.ok) {
//                throw new Error('Network response was not ok');
//            }
//        });
//}

//Затычка для отсутствия ошибок в консоли
function trackVisitor(visualNovelId) {}



function loadTagsMetadata(visualNovelId, spoilerLevel) {
    var alert = document.getElementById("liveAlertPlaceholder");

    var btnNone = document.getElementById('btnNoneSpoilerLevel');
    var btnMinor = document.getElementById('btnMinorSpoilerLevel');
    var btnMajor = document.getElementById('btnMajorSpoilerLevel');

    switch (spoilerLevel) {
        case 1:
            btnNone.disabled = false;
            btnMinor.disabled = true;
            btnMajor.disabled = false;
            break;
        case 2:
            btnNone.disabled = false;
            btnMinor.disabled = false;
            btnMajor.disabled = true;
            break;
        default:
            btnNone.disabled = true;
            btnMinor.disabled = false;
            btnMajor.disabled = false;
            break;
    }

    alert.innerHTML = `<div class="spinner-border my-2" role = "status"><span class="visually-hidden">Loading...</span></div>`;

    var url = "/Novel/Tags?visualNovelId=" + visualNovelId + "&spoilerLevel=" + spoilerLevel;

    var xhr = new XMLHttpRequest();
    xhr.open("GET", url, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                if (xhr.responseText.length > 2) {
                    document.getElementById("tags-column-container").innerHTML = xhr.responseText;
                } else {
                    alert.innerHTML = `@await Html.PartialAsync("_ErrorAlert")`;
                }
                alert.innerHTML = '';
            } else {
                alert.innerHTML = `@await Html.PartialAsync("_ErrorAlert")`;
            }
        }
    };
    xhr.send();
}

function loadToast(message, isSuccessToast) {
    var xhr = new XMLHttpRequest();
    if (isSuccessToast) {
        var url = "/Notification/SuccessToast?message=" + message;
    }
    else {
        var url = "/Notification/ErrorToast?message=" + message;
    }
    var notificationContainer = document.getElementById("notification-container");
    xhr.open("GET", url, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            notificationContainer.innerHTML = xhr.responseText;
            var toast = document.getElementById('notification-toast');
            let toastBootstrap = bootstrap.Toast.getOrCreateInstance(toast);
            toastBootstrap.show();
        }
    };
    xhr.send();
}

//End Novel Page

//Novel Page Labels

function enableLabelsSelect(userId, visualNovelId) {
    var customSelectWrapper = document.getElementById('custom-select-wrapper');
    var customSelectOptions = document.getElementById('custom-select-options');
    var newOptionInput = document.getElementById('new-option-input');
    var addOptionBtn = document.getElementById('add-option-btn');
    var customSelectContainer = document.getElementById('custom-select-container');

    var isAdding = false; // Флаг для предотвращения двойных добавлений

    //var checkboxes = customSelectOptions.getElementsByTagName('input');
    //for (var i = 0; i < checkboxes.length; i++) {
    //    if (checkboxes[i].dataset.mutuallyExclusive === 'true' && checkboxes[i].checked === true) {
    //        toggleMutuallyExclusive(checkboxes[i]);
    //        break;
    //    }
    //}

    customSelectWrapper.addEventListener('click', function () {
        customSelectOptions.style.display = customSelectOptions.style.display === 'block' ? 'none' : 'block';
    });

    customSelectOptions.addEventListener('click', function (event) {
        if (event.target.tagName === 'INPUT') {
            handleCheckboxChange(event.target);
            updateSelectValue();
        }
    });

    addOptionBtn.addEventListener('click', function () {
        var newOptionText = newOptionInput.value.trim();

        if (newOptionText && !isAdding) {
            isAdding = true; // Установить флаг
            var optionExists = false;
            var optionValue = null;

            // Проверка, существует ли уже такая опция
            var checkboxes = customSelectOptions.getElementsByTagName('input');
            for (var i = 0; i < checkboxes.length; i++) {
                if (checkboxes[i].nextSibling.nodeValue.trim().toLowerCase() === newOptionText.toLowerCase()) {
                    optionExists = true;
                    optionValue = checkboxes[i].value;
                    break;
                }
            }

            if (optionExists) {
                // Если опция уже выбрана, сообщаем об этом и не добавляем
                if (checkboxes[i].checked) {
                    loadToast('Новелла уже добавленна в этот список', false);
                } else {
                    // Выбираем существующую опцию
                    checkboxes[i].checked = true;
                    
                    handleCheckboxChange(checkboxes[i]);
                    updateSelectValue();
                }
                isAdding = false; // Сбросить флаг
            } else {
                var isPrivate = false;

                // Получаем значение текущего домена
                var domain = window.location.origin;

                // Полный URL для запроса
                var url = domain + '/User/CreateCustomList?userId=' + userId + '&listName=' + encodeURIComponent(newOptionText) + '&isPrivate=' + isPrivate;

                // Отправляем GET-запрос к API
                fetch(url, {
                    method: 'GET'
                })
                    .then(responce => responce)
                    .then(data => {
                        if (data.ok) {
                            // Создаем новый чекбокс
                            var newCheckbox = document.createElement('input');
                            newCheckbox.type = 'checkbox';
                            newCheckbox.value = data.id;
                            newCheckbox.checked = true;

                            var newLabel = document.createElement('label');
                            newLabel.appendChild(newCheckbox);
                            newLabel.appendChild(document.createTextNode(' ' + newOptionText));
                            newLabel.className = 'option-item';

                            // Добавляем новый чекбокс в контейнер
                            customSelectOptions.appendChild(newLabel);
                            
                            handleCheckboxChange(checkboxes[i]);
                            updateSelectValue();
                        } else {
                            loadToast('Неверный ответ от сервера', false);
                        }
                        isAdding = false; // Сбросить флаг
                    })
                    .catch(error => {
                        console.error('Ошибка AJAX-запроса:', error);
                        loadToast('Произошла ошибка при добавлении нового списка', false);
                    });
            }
        }
    });

    //function handleResize() {
    //    if (window.innerWidth <= 768) {
    //        customSelectWrapper.classList.add('hidden');
    //        customSelectOptions.classList.add('hidden');

    //        if (!document.getElementById('nativeSelect')) {
    //            var nativeSelect = document.createElement('select');
    //            nativeSelect.id = 'nativeSelect';
    //            nativeSelect.multiple = true;
    //            customSelectContainer.appendChild(nativeSelect);

    //            var checkboxes = customSelectOptions.getElementsByTagName('input');
    //            for (var i = 0; i < checkboxes.length; i++) {
    //                var option = document.createElement('option');
    //                option.value = checkboxes[i].value;
    //                option.text = checkboxes[i].nextSibling.nodeValue.trim();
    //                option.selected = checkboxes[i].checked;
    //                nativeSelect.appendChild(option);
    //            }

    //            nativeSelect.addEventListener('change', function (event) {
    //                var selectedOptions = Array.from(nativeSelect.options).filter(option => option.selected);

    //                selectedOptions.forEach(function (option) {
    //                    var correspondingCheckbox = Array.from(checkboxes).find(checkbox => checkbox.value === option.value);
    //                    if (correspondingCheckbox && !correspondingCheckbox.checked) {
    //                        correspondingCheckbox.checked = true;
    //                        handleCheckboxChange(correspondingCheckbox);
    //                    }
    //                });

    //                Array.from(checkboxes).forEach(function (checkbox) {
    //                    if (!selectedOptions.some(option => option.value === checkbox.value)) {
    //                        if (checkbox.checked) {
    //                            checkbox.checked = false;
    //                            handleCheckboxChange(checkbox);
    //                        }
    //                    }
    //                });
    //            });
    //        }
    //    } else {
    //        customSelectWrapper.classList.remove('hidden');
    //        customSelectOptions.classList.remove('hidden');

    //        var nativeSelect = document.getElementById('nativeSelect');
    //        if (nativeSelect) {
    //            customSelectContainer.removeChild(nativeSelect);
    //        }
    //    }
    //}

    function updateSelectValue() {
        var checkboxes = customSelectOptions.getElementsByTagName('input');
        var selectedValues = [];
        var selectedLabels = [];

        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].checked) {
                selectedValues.push(checkboxes[i].value);
                selectedLabels.push(checkboxes[i].nextSibling.nodeValue.trim());
            }
        }

        customSelectWrapper.textContent = selectedLabels.length > 0 ? selectedLabels.join(', ') : 'Добавить в список';

        //console.log('Selected values:', selectedValues);
    }

    //function updateNativeSelect() {
    //    var nativeSelect = document.getElementById('nativeSelect');
    //    if (nativeSelect) {
    //        var checkboxes = customSelectOptions.getElementsByTagName('input');
    //        nativeSelect.innerHTML = ''; // Очистить существующие опции

    //        for (var i = 0; i < checkboxes.length; i++) {
    //            var option = document.createElement('option');
    //            option.value = checkboxes[i].value;
    //            option.text = checkboxes[i].nextSibling.nodeValue.trim();
    //            option.selected = checkboxes[i].checked;
    //            nativeSelect.appendChild(option);
    //        }
    //    }
    //}

    function toggleMutuallyExclusive(selectedCheckbox) {
        var checkboxes = customSelectOptions.getElementsByTagName('input');
        var checkArray = Array.from(checkboxes);
        let result = checkArray.map(item => item.dataset.mutuallyExclusive === 'true' && item.checked);
        const count = result.filter(Boolean).length;
        if (selectedCheckbox.dataset.mutuallyExclusive === 'true' && count >= 2) {
            for (var i = 0; i < checkboxes.length; i++) {
                if (checkboxes[i] !== selectedCheckbox && checkboxes[i].dataset.mutuallyExclusive === 'true' && checkboxes[i].checked) {
                    console.log("Mutually Exclusive:" + checkboxes[i].value);
                    checkboxes[i].checked = !checkboxes[i].checked;
                    handleCheckboxChange(checkboxes[i]);
                }
            }
        }
    }

    function handleCheckboxChange(checkbox) {
        var domain = window.location.origin;

        blockOptionsSelect();

        if (checkbox.checked) {
            var url = domain + '/User/AddToUserList?userId=' + userId + '&listId=' + checkbox.value + '&visualNovelId=' + visualNovelId;
            fetch(url, {
                method: 'POST'
            })
                .then(response => {
                    if (!response.ok) {
                        loadToast('Ошибка сети', false);
                        unblockOptionsSelect();
                    }
                   
                    return response;
                })
                .then(data => {
                    if (data.ok) {
                        unblockOptionsSelect();
                        //updateNativeSelect();
                        toggleMutuallyExclusive(checkbox);
                        //loadToast('Добавленно в список', true);
                        updateSelectValue();
                        
                        
                    } else {
                        loadToast('Ошибка добавления в список', false);
                        checkbox.checked = !checkbox.checked; // откат изменения
                        unblockOptionsSelect();
                    }
                })
                .catch(error => {
                    console.error('Ошибка:', error);
                    loadToast('Ошибка: ' + error, false);
                    checkbox.checked = !checkbox.checked; // откат изменения
                    unblockOptionsSelect();
                });
            
        } else {
            var url = domain + '/User/RemoveFromUserList?userId=' + userId + '&listId=' + checkbox.value + '&visualNovelId=' + visualNovelId;
            fetch(url, {
                method: 'DELETE'
            })
                .then(response => {
                    if (!response.ok) {
                        loadToast('Ошибка сети', false);
                        unblockOptionsSelect();
                    }
                    return response;
                })
                .then(data => {
                    if (data.ok) {
                        unblockOptionsSelect();
                        //updateNativeSelect();
                        toggleMutuallyExclusive(checkbox);
                        //loadToast('Удалено из списка', true);
                        updateSelectValue();
                        
                    } else {
                        loadToast('Ошибка удаления из списка', false);
                        checkbox.checked = !checkbox.checked; // откат изменения
                        unblockOptionsSelect();
                    }
                })
                .catch(error => {
                    console.error('Ошибка:', error);
                    loadToast('Ошибка: ' + error, false);
                    checkbox.checked = !checkbox.checked; // откат изменения
                    unblockOptionsSelect();
                });
        }

        
    }

    function blockOptionsSelect() {
        //var spinnerDiv = document.createElement('div');
        //spinnerDiv.setAttribute('class', 'spinner-border position-absolute top-50 start-50 translate-middle');
        //spinnerDiv.setAttribute('role', 'status');
        //spinnerDiv.setAttribute('id', 'blocked-spinner');
        //var spinnerSpan = document.createElement('span');
        //spinnerSpan.setAttribute('class', 'visually-hidden');
        //spinnerSpan.innerText = "Загрузка...";
        //spinnerDiv.appendChild(spinnerSpan);
        //customSelectOptions.appendChild(spinnerDiv);
        customSelectOptions.classList.add('block');
    }

    function unblockOptionsSelect() {
        //var spinner = document.getElementById('blocked-spinner');
        //spinner.parentElement.removeChild(spinner);
        customSelectOptions.classList.remove('block');
    }

    //window.addEventListener('resize', handleResize);
    //handleResize();

    // Закрытие кастомного селекта при клике вне его области
    document.addEventListener('click', function (event) {
        if (!customSelectContainer.contains(event.target) && !customSelectWrapper.contains(event.target)) {
            customSelectOptions.style.display = 'none';
        }
    });

    updateSelectValue();
    //updateNativeSelect();
}

//End Novel Page Labesl

//Comments

function enableToggleSpoilers() {
    var spoilers = document.querySelectorAll('.spoiler');
    spoilers.forEach(function (spoiler) {
        spoiler.onclick = function () {
            toggleSpoiler(this);
        };
    });
}

function clearReplyForm(commentId) {
    var textarea = document.getElementById('commentText-' + commentId);
    textarea.value = "";
}

function showReplyForm(commentId) {
    var form = document.getElementById('replyFormContainer-' + commentId);
    form.style.display = 'block';
}

function hideReplyForm(commentId) {
    var form = document.getElementById('replyFormContainer-' + commentId);
    form.style.display = 'none';
}

function showUpdateForm(commentId) {
    var form = document.getElementById('updateFormContainer-' + commentId);
    form.style.display = 'block';
}

function hideUpdateForm(commentId) {
    var form = document.getElementById('updateFormContainer-' + commentId);
    form.style.display = 'none';
}

function formatText(format, id) {
    var textarea = document.getElementById('commentText-' + id);
    var start = textarea.selectionStart;
    var end = textarea.selectionEnd;
    var selectedText = textarea.value.substring(start, end);

    var formattedText = '';
    switch (format) {
        case 'bold':
            formattedText = `**${selectedText}**`;
            break;
        case 'italic':
            formattedText = `*${selectedText}*`;
            break;
        case 'underline':
            formattedText = `__${selectedText}__`;
            break;
        case 'spoiler':
            formattedText = `||${selectedText}||`;
            break;
    }

    textarea.value = textarea.value.substring(0, start) + formattedText + textarea.value.substring(end);
    textarea.focus();
    textarea.selectionStart = start;
    textarea.selectionEnd = start + formattedText.length;
}

function toggleSpoiler(element) {
    element.classList.toggle('revealed');
}

function submitAddComment() {
    var form = document.getElementById('addForm');
    var formData = new FormData(form);

    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/Comment/Add', true);
    xhr.onload = function () {
        if (xhr.status === 200) {
            // Add the new comment to the UI
            var newComment = document.createElement('div');
            newComment.innerHTML = xhr.responseText;
            document.getElementById('comment-container').prepend(newComment);
            var textarea = document.getElementById('commentText-newComment');
            textarea.value = "";

            var commentsCount = document.getElementById('comments-count');
            var count = Number(commentsCount.innerHTML);
            count++;
            commentsCount.innerHTML = count;

            enableToggleSpoilers();
        } else {
            loadToast("Ошибка добавления комментария", false);
            console.error('Error add comment:', xhr.statusText);
        }
    };
    xhr.send(formData);
}

function submitReplyComment(id) {
    var form = document.getElementById('replyForm-' + id);
    var formData = new FormData(form);

    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/Comment/Reply', true);
    xhr.onload = function () {
        if (xhr.status === 200) {
            var newComment = document.createElement('div');
            newComment.innerHTML = xhr.responseText;
            if (document.getElementById('replies-' + id)) {
                document.getElementById('replies-' + id).prepend(newComment);
            }
            else {
                var replyContainer = document.createElement('div');
                replyContainer.setAttribute("id", "replies-" + id);
                replyContainer.setAttribute("class", "replies");
                document.getElementById('comment-' + id).appendChild(replyContainer);

                document.getElementById('replies-' + id);
                document.getElementById('replies-' + id).prepend(newComment);
            }

            var commentsCount = document.getElementById('comments-count');
            var count = Number(commentsCount.innerHTML);
            count++;
            commentsCount.innerHTML = count;

            clearReplyForm(id);
            hideReplyForm(id);
            enableToggleSpoilers();
        } else {
            loadToast("Ошибка ответа на комментарий", false);
            console.error('Error reply comment:', xhr.statusText);
        }
    };
    xhr.send(formData);
}

function submitUpdateComment(id) {
    var form = document.getElementById('updateForm-' + id);
    var formData = new FormData(form);

    var xhr = new XMLHttpRequest();
    xhr.open('PUT', '/Comment/Update', true);
    xhr.onload = function () {
        if (xhr.status === 200) {
            var commentContent = document.getElementById('comment-content-' + id);
            
            commentContent.innerHTML = xhr.responseText;

            enableToggleSpoilers();
            hideUpdateForm(id);
        } else {
            loadToast("Ошибка при изменении комментария", false);
            console.error('Error update comment:', xhr.statusText);
        }
    };
    xhr.send(formData);
}

function submitDeleteComment(id) {
    var comment = document.getElementById('comment-' + id);
    console.log(id);
    console.log(comment);
    var xhr = new XMLHttpRequest();
    xhr.open('DELETE', '/Comment/Delete?id=' + id, true);
    xhr.onload = function () {
        if (xhr.status === 200) {
            var replies = document.getElementById('replies-' + id);
            if (replies) {
                var commentMenu = document.getElementById('comment-menu-' + id);
                commentMenu.parentElement.removeChild(commentMenu);
                var commentContent = document.getElementById('comment-content-' + id);
                commentContent.classList.add("deleted-comment");
                loadToast("Комментарий удален", true);
            } else {
                comment.parentElement.removeChild(comment);
                loadToast("Комментарий удален", true);
            }

            var commentsCount = document.getElementById('comments-count');
            var count = Number(commentsCount.innerHTML);
            count--;
            commentsCount.innerHTML = count;

        } else {
            loadToast("Ошибка при удалении комментария", false);
            console.error('Error delete comment:', xhr.statusText);
        }
    };
    console.log(xhr);
    xhr.send();
}

function getUserRating(userId, commentId, callback) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', '/Comment/GetRating?userId=' + userId + '&commentId=' + commentId, true);

    xhr.onload = function () {
        if (xhr.status === 200) {
            var responce = JSON.parse(xhr.responseText);
            callback(true, responce);
        } else {
            callback(false, null);
        }
    };
    xhr.send();
}

function handleRating(userId, commentId, isLike) {

    getUserRating(userId, commentId, function (isRatingExist, responce) {
        if (isRatingExist) {
            if (responce.isLike == isLike) {
                var xhr = new XMLHttpRequest();
                xhr.open('DELETE', '/Comment/RemoveRating?userId=' + userId + '&commentId=' + commentId, true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        var ratingCounts;
                        if (isLike) {
                            var like = document.getElementById('like-' + commentId);
                            like.classList.remove("comment-rating-icon-active");

                            ratingCounts = document.getElementById('likes-' + commentId);
                            var count = Number(ratingCounts.innerHTML);
                            count--;
                            ratingCounts.innerHTML = count > 0 ? count : "";
                        } else {
                            var dislike = document.getElementById('dislike-' + commentId);
                            dislike.classList.remove("comment-rating-icon-active");

                            ratingCounts = document.getElementById('dislikes-' + commentId);
                            var count = Number(ratingCounts.innerHTML);
                            count--;
                            ratingCounts.innerHTML = count > 0 ? count : "";
                        }
                    } else {
                        loadToast("Ошибка удаления рейтинга комментария", false);
                        console.error('Error delete rating:', xhr.statusText);
                    }
                };
                xhr.send();
            } else {
                var xhr = new XMLHttpRequest();
                xhr.open('PUT', '/Comment/UpdateRating?userId=' + userId + '&commentId=' + commentId + '&isLike=' + !responce.isLike, true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        if (isLike) {
                            likesRatingCounts = document.getElementById('likes-' + commentId);
                            var likesCount = Number(likesRatingCounts.innerHTML);
                            likesCount++;
                            likesRatingCounts.innerHTML = likesCount > 0 ? likesCount : "";

                            dislikesRatingCounts = document.getElementById('dislikes-' + commentId);
                            var dislikesCount = Number(dislikesRatingCounts.innerHTML);
                            dislikesCount--;
                            dislikesRatingCounts.innerHTML = dislikesCount > 0 ? dislikesCount : "";

                            var like = document.getElementById('like-' + commentId);
                            like.classList.add("comment-rating-icon-active");

                            var dislike = document.getElementById('dislike-' + commentId);
                            dislike.classList.remove("comment-rating-icon-active");
                        } else {
                            likesRatingCounts = document.getElementById('likes-' + commentId);
                            var likesCount = Number(likesRatingCounts.innerHTML);
                            likesCount--;
                            likesRatingCounts.innerHTML = likesCount > 0 ? likesCount : "";

                            dislikesRatingCounts = document.getElementById('dislikes-' + commentId);
                            var dislikesCount = Number(dislikesRatingCounts.innerHTML);
                            dislikesCount++;
                            dislikesRatingCounts.innerHTML = dislikesCount > 0 ? dislikesCount : "";

                            var like = document.getElementById('like-' + commentId);
                            like.classList.remove("comment-rating-icon-active");
                            var dislike = document.getElementById('dislike-' + commentId);
                            dislike.classList.add("comment-rating-icon-active");
                        }
                    } else {
                        loadToast("Ошибка изменения рейтинга к комментарию", false);
                        console.error('Error update rating to comment:', xhr.statusText);
                    }
                };
                xhr.send();
            }
        } else {
            var xhr = new XMLHttpRequest();
            xhr.open('POST', '/Comment/AddRating?userId=' + userId + '&commentId=' + commentId + '&isLike=' + isLike, true);
            xhr.onload = function () {
                if (xhr.status === 200) {
                    var ratingCounts;
                    if (isLike) {
                        var like = document.getElementById('like-' + commentId);
                        like.classList.add("comment-rating-icon-active");
                        ratingCounts = document.getElementById('likes-' + commentId);
                        var count = Number(ratingCounts.innerHTML);
                        count++;
                        ratingCounts.innerHTML = count;
                    } else {
                        var dislike = document.getElementById('dislike-' + commentId);
                        dislike.classList.add("comment-rating-icon-active");
                        ratingCounts = document.getElementById('dislikes-' + commentId);
                        var count = Number(ratingCounts.innerHTML);
                        count++;
                        ratingCounts.innerHTML = count;
                    }
                } else {
                    loadToast("Ошибка добавления рейтинга к комментарию", false);
                    console.error('Error add rating to comment:', xhr.statusText);
                }
            };
            xhr.send();
        }
    });
}

//End Comments

//Novels Page

function getFilters() {
    var tags = [];
    var excludeTags = [];
    var genres = [];
    var excludeGenres = [];
    var languages = [];
    var excludeLanguages = [];
    var platforms = [];
    var excludePlatforms = [];
    var search = "";

    var spoilerLevel = document.querySelector('input[name="spoilerLevelCheck"]:checked').value;
    var readingTime = document.getElementById('readingTimeSelect').value;
    var sort = document.getElementById('sortSelect').value;

    document.querySelectorAll('input[name="tags"]').forEach(function (tag) {
        var tagName = tag.value;
        var tagState = tag.getAttribute('data-state');
        if (tagState === 'include') {
            tags.push(tagName);
        } else if (tagState === 'exclude') {
            excludeTags.push(-tagName);
        }
    });

    document.querySelectorAll('input[name="genres"]').forEach(function (genre) {
        var genreName = genre.value;
        var genreState = genre.getAttribute('data-state');
        if (genreState === 'include') {
            genres.push(genreName);
        } else if (genreState === 'exclude') {
            excludeGenres.push(-genreName);
        }
    });

    document.querySelectorAll('input[name="languages"]').forEach(function (language) {
        var languageName = language.value;
        var languageState = language.getAttribute('data-state');
        if (languageState === 'include') {
            languages.push(languageName);
        } else if (languageState === 'exclude') {
            excludeLanguages.push(-languageName);
        }
    });

    document.querySelectorAll('input[name="platforms"]').forEach(function (platform) {
        var platformName = platform.value;
        var platformState = platform.getAttribute('data-state');
        if (platformState === 'include') {
            platforms.push(platformName);
        } else if (platformState === 'exclude') {
            excludePlatforms.push(-platformName);
        }
    });

    search = document.getElementById('visual-novel-search-input').value;

    return {
        tags: tags.concat(excludeTags),
        genres: genres.concat(excludeGenres),
        //authors: authors.concat(),
        //translators: document.getElementById('translators').value,
        languages: languages.concat(excludeLanguages),
        platforms: platforms.concat(excludePlatforms),
        spoilerLevel: spoilerLevel,
        readingTime: readingTime,
        sort, sort,
        search, search
    };
}

function setFilters(filters) {
    let genres = stringToArray(filters.genres, ',');

    genres.forEach(function (filter) {
        document.querySelectorAll('input[name="genres"]').forEach(function (genre) {
            if (genre.value == Math.abs(filter)) {
                if (filter > 0) {
                    genre.setAttribute('data-state', 'include');
                    genre.parentNode.classList.add('checked');
                }
                else {
                    genre.setAttribute('data-state', 'exclude');
                    genre.parentNode.classList.add('excluded');
                }
            }
        });
    });

    let tags = stringToArray(filters.tags, ',');

    tags.forEach(function (filter) {
        document.querySelectorAll('input[name="tags"]').forEach(function (tag) {
            if (tag.value == Math.abs(filter)) {
                if (filter > 0) {
                    tag.setAttribute('data-state', 'include');
                    tag.parentNode.classList.add('checked');
                }
                else {
                    tag.setAttribute('data-state', 'exclude');
                    tag.parentNode.classList.add('excluded');
                }
            }
        });
    });

    let languages = stringToArray(filters.languages, ',');

    languages.forEach(function (filter) {
        document.querySelectorAll('input[name="languages"]').forEach(function (language) {
            if (language.value == Math.abs(filter)) {
                if (filter > 0) {
                    language.setAttribute('data-state', 'include');
                    language.parentNode.classList.add('checked');
                }
                else {
                    language.setAttribute('data-state', 'exclude');
                    language.parentNode.classList.add('excluded');
                }
            }
        });
    });

    let platforms = stringToArray(filters.platforms, ',');

    platforms.forEach(function (filter) {
        document.querySelectorAll('input[name="platforms"]').forEach(function (platform) {
            if (platform.value == Math.abs(filter)) {
                if (filter > 0) {
                    platform.setAttribute('data-state', 'include');
                    platform.parentNode.classList.add('checked');
                }
                else {
                    platform.setAttribute('data-state', 'exclude');
                    platform.parentNode.classList.add('excluded');
                }
            }
        });
    });

    // var spoilerLevel = document.querySelector('input[name="spoilerLevelCheck"]:checked').value;
    // var readingTime = document.getElementById('readingTimeSelect').value;
    // var sort = document.getElementById('sortSelect').value;

    document.querySelector('input[name="spoilerLevelCheck"]:checked').value = filters.spoilerLevel;
    document.getElementById('readingTimeSelect').value = filters.readingTime;
    document.getElementById('sortSelect').value = filters.sort;
    document.getElementById('visual-novel-search-input').value = filters.search;
}

function stringToArray(string, separator) {
    if (string) {
        return string.split(separator).map(Number);
    }
    return [];
}

function changePage(page, filters, pushToHistory = true) {
    if (document.querySelector('.btn-primary-submit-filter')) {
        var { tags, genres, languages, platforms, spoilerLevel, readingTime, sort, search } = getFilters();

        // if (!filters) {
        //     var urlParams = new URLSearchParams(window.location.search);
        //     filters = {
        //         tags: urlParams.get('tags') || '',
        //         genres: urlParams.get('genres') || '',
        //         //authors: urlParams.get('authors') || '',
        //         //translators: urlParams.get('translators') || '',
        //         languages: urlParams.get('languages') || '',
        //         platforms: urlParams.get('platforms') || '',
        //         spoilerLevel: urlParams.get('spoilerLevel') || '',
        //         readingTime: urlParams.get('readingTime') || '',
        //         sort: urlParams.get('sort') || ''
        //     };
        // }

        if (pushToHistory) {
            updateUrl(page, filters);
        }

        // if (pushToHistory) {
        //     history.pushState({ page: page }, '', `?page=${page}`);
        // }

        let itemsPerPage = 20;

        //window.scrollTo(0, 0);

        updateContent(tags, genres, languages, platforms, spoilerLevel, readingTime, sort, search, page, itemsPerPage);
    }
}

function updateUrl(page, filters) {
    const url = new URL(window.location.href);
    url.searchParams.set('page', page);

    for (const key in filters) {
        if (filters[key] && filters[key] != '') {
            url.searchParams.set(key, filters[key]);
        } else {
            url.searchParams.delete(key);
        }
    }

    history.pushState({ page: page, filters: filters }, '', url.toString());

    // const params = new URLSearchParams({ ...filters, page: page });

    // const newUrl = `${window.location.pathname}?${params.toString()}`;

    // history.pushState({ page: page, filters: filters }, '', newUrl);
}

//End Novels Page

//Profile Page

//function createCustomList() {
//    var form = document.getElementById('create-visual-novel-list-form');
//    var formData = new FormData(form);
//    var domain = window.location.origin;
//    var listsCollapse = document.getElementById('edit-novel-lists-collapse');

//    var xhr = new XMLHttpRequest();
//    xhr.open('POST', `${domain}/User/CreateCustomList`, true);
//    xhr.onload = function () {
//        if (xhr.status === 200) {
//        } else {
//            loadToast("Ошибка добавления комментария", false);
//            console.error('Error add comment:', xhr.statusText);
//        }
//    };
//    xhr.send(formData);
//}

function editVisualNovelListModal(element) {
    var id = element.dataset.listId;
    var name = element.dataset.listName;
    var isPrivate = element.dataset.listIsPrivate;
    var isCustom = element.dataset.listIsCustom;

    var listNameInput = document.getElementById('edit-novel-list-name');
    var listIdInput = document.getElementById('edit-novel-list-id');
    var isPrivateInput = document.getElementById('edit-novel-list-is-private');
    var isCustomInput = document.getElementById('edit-novel-list-is-custom');

    listIdInput.value = id;
    listNameInput.value = name;

    if (isPrivate == "true") {
        isPrivateInput.checked = true;
    } else {
        isPrivateInput.checked = false;
    }
    if (isCustom == "true") {
        console.log(isCustom);
        listNameInput.removeAttribute("readonly");
        isCustomInput.checked = true;
    } else {
        console.log(isCustom);
        listNameInput.setAttribute("readonly", "true");
        isCustomInput.checked = false;
    }
}

function deleteVisualNovelList(element) {
    var domain = window.location.origin;
    var id = element.dataset.listId;
    var parentRow = document.getElementById('row-edit-novels-list-' + id);
    

    var xhr = new XMLHttpRequest();
    xhr.open('DELETE', `${domain}/User/DeleteVisualNovelList?listId=` + id, true);

    xhr.onload = function () {
        if (xhr.status === 200) {
            parentRow.remove();
        } else {
            loadToast("Ошибка при удалении списка", false);
        }
    };
    xhr.send();
}

//End Profile Page