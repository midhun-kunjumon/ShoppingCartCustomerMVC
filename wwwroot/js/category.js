$(document).ready(function () {

    loadCategories();

});


function loadCategories() {

    $.ajax({

        url: '/api/CategoryApi',

        type: 'GET',

        success: function (categories) {

            var container = $('#category-container');

            container.empty();


            if (!categories || categories.length === 0) {

                container.html(
                    '<div class="text-center py-5">' +
                    '<p>No categories available.</p>' +
                    '</div>'
                );

                return;
            }


            $.each(categories, function (index, category) {

                var imageUrl = category.imageUrl
                    ? category.imageUrl
                    : '/images/category-placeholder.png';


                var categoryHtml =

                    '<div class="category-item mb-4">' +

                    '<div class="category-card category-toggle" ' +
                    'data-category-id="' +
                    category.categoryID +
                    '">' +

                    '<div class="category-image">' +

                    '<img src="' +
                    imageUrl +
                    '" ' +
                    'alt="' +
                    escapeHtml(category.categoryName) +
                    '" />' +

                    '</div>' +

                    '<div class="category-content">' +

                    '<h5>' +
                    escapeHtml(category.categoryName) +
                    '</h5>' +

                    '<span class="category-arrow">' +
                    '▼' +
                    '</span>' +

                    '</div>' +

                    '</div>' +


                    '<div class="subcategory-container" ' +
                    'id="subcategory-' +
                    category.categoryID +
                    '" ' +
                    'style="display:none;">' +

                    '</div>' +

                    '</div>';


                container.append(categoryHtml);

            });


            /*
             * After all categories are loaded,
             * check whether a category was selected
             * from Home or Navbar.
             */
            openSelectedCategory();

        },

        error: function (xhr, status, error) {

            console.error(
                'Error loading categories:',
                error
            );

            $('#category-container').html(

                '<div class="text-center py-5">' +
                '<p>Unable to load categories.</p>' +
                '</div>'

            );

        }

    });

}


/*
 * =========================================================
 * CATEGORY CARD CLICK
 * =========================================================
 *
 * Clicking a category opens its subcategories.
 */
$(document).on('click', '.category-toggle', function () {

    var categoryCard = $(this);

    var categoryId =
        categoryCard.attr('data-category-id');

    var subcategoryContainer =
        $('#subcategory-' + categoryId);


    if (!subcategoryContainer.length) {
        return;
    }


    /*
     * Close all other category dropdowns.
     */
    $('.subcategory-container')
        .not(subcategoryContainer)
        .slideUp();


    /*
     * Reset all other arrows.
     */
    $('.category-toggle')
        .not(categoryCard)
        .find('.category-arrow')
        .css('transform', 'rotate(0deg)');


    /*
     * If this category is already open,
     * close it.
     */
    if (subcategoryContainer.is(':visible')) {

        subcategoryContainer.slideUp();

        categoryCard
            .find('.category-arrow')
            .css('transform', 'rotate(0deg)');

        return;

    }


    /*
     * If subcategories were already loaded,
     * simply show them.
     */
    if (subcategoryContainer.data('loaded') === true) {

        subcategoryContainer.slideDown();

        categoryCard
            .find('.category-arrow')
            .css('transform', 'rotate(180deg)');

        return;

    }


    /*
     * Otherwise load subcategories.
     */
    loadSubCategories(
        categoryId,
        subcategoryContainer,
        categoryCard
    );

});


/*
 * =========================================================
 * LOAD SUBCATEGORIES
 * =========================================================
 */
function loadSubCategories(
    categoryId,
    subcategoryContainer,
    categoryCard
) {

    subcategoryContainer.html(

        '<div class="py-3">' +
        '<span>Loading subcategories...</span>' +
        '</div>'

    );


    subcategoryContainer
        .stop(true, true)
        .slideDown();


    $(categoryCard)
        .find('.category-arrow')
        .css('transform', 'rotate(180deg)');


    $.ajax({

        url:
            '/api/SubCategoryApi/category/' +
            categoryId,

        type: 'GET',

        success: function (subCategories) {

            subcategoryContainer.empty();


            if (!subCategories ||
                subCategories.length === 0) {

                subcategoryContainer.html(

                    '<div class="py-3">' +
                    '<p>No subcategories available.</p>' +
                    '</div>'

                );

                /*
                 * Mark as loaded even when empty
                 * so we don't repeatedly call the API.
                 */
                subcategoryContainer.data(
                    'loaded',
                    true
                );

                return;

            }


            $.each(
                subCategories,
                function (index, subCategory) {

                    var subCategoryHtml =

                        '<a href="/Product/SubCategory/' +
                        subCategory.subCategoryID +
                        '" ' +
                        'class="subcategory-link" ' +
                        'data-subcategory-id="' +
                        subCategory.subCategoryID +
                        '">' +

                        '<span>' +

                        escapeHtml(
                            subCategory.subCategoryName
                        ) +

                        '</span>' +

                        '</a>';


                    subcategoryContainer.append(
                        subCategoryHtml
                    );

                }
            );


            subcategoryContainer.data(
                'loaded',
                true
            );

        },

        error: function (xhr, status, error) {

            console.error(
                'Error loading subcategories:',
                error
            );

            subcategoryContainer.html(

                '<div class="py-3">' +
                '<p>Unable to load subcategories.</p>' +
                '</div>'

            );

        }

    });

}


/*
 * =========================================================
 * OPEN SELECTED CATEGORY
 * =========================================================
 *
 * Used when arriving at:
 *
 * /Category/Index?categoryId=5
 *
 * This is used by:
 *
 * 1. Navbar category
 * 2. Home Shop by Category card
 */
function openSelectedCategory() {

    var urlParams =
        new URLSearchParams(
            window.location.search
        );


    var categoryId =
        urlParams.get('categoryId');


    /*
     * No category was selected.
     */
    if (!categoryId) {
        return;
    }


    /*
     * Convert to number.
     */
    categoryId =
        parseInt(categoryId, 10);


    if (isNaN(categoryId)) {
        return;
    }


    /*
     * Find category card.
     */
    var categoryCard =
        $('.category-toggle').filter(function () {

            return String(
                $(this).attr('data-category-id')
            ) === String(categoryId);

        });


    if (categoryCard.length === 0) {

        console.warn(
            'Category not found:',
            categoryId
        );

        return;

    }


    /*
     * Find its subcategory container.
     */
    var subcategoryContainer =
        $('#subcategory-' + categoryId);


    if (subcategoryContainer.length === 0) {
        return;
    }


    /*
     * Highlight selected category.
     */
    categoryCard.addClass(
        'selected-category'
    );


    /*
     * Open and load its subcategories.
     */
    loadSubCategories(
        categoryId,
        subcategoryContainer,
        categoryCard
    );


    /*
     * Scroll to the selected category.
     */
    setTimeout(function () {

        var offset =
            categoryCard.offset();

        if (!offset) {
            return;
        }


        $('html, body').animate({

            scrollTop:
                offset.top - 120

        }, 400);

    }, 500);

}


/*
 * =========================================================
 * HTML ESCAPING
 * =========================================================
 */
function escapeHtml(value) {

    return $('<div>')
        .text(value || '')
        .html();

}