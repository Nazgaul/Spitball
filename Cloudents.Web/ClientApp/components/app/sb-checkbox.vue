<template functional >
    <div class="container2">
        <input :id="props.id" type="checkbox" :checked="props.checked" @change="props.callback">
        <span class="checkmark"></span>
    </div>
</template>

<style>

    /* The container */
    .container2 {
        display: block;
        position: relative;
        padding-left: 24px;
        margin-bottom: 12px;
        cursor: pointer;
        -webkit-user-select: none;
        -moz-user-select: none;
        -ms-user-select: none;
        user-select: none;
    }

        /* Hide the browser's default checkbox */
        .container2 input {
            position: absolute;
            opacity: 0;
        }

    /* Create a custom checkbox */
    .checkmark {
        position: absolute;
        top: 0;
        left: 0;
        height: 15px;
        width: 15px;
        background-color: #d0cbcb;
    }

    /* On mouse-over, add a grey background color */
    .container2:hover input ~ .checkmark {
        background-color: #a7a5a5;
    }

    /* When the checkbox is checked, add a blue background */
    .container2 input:checked ~ .checkmark {
        background-color: #3293f6;
    }

    /* Create the checkmark/indicator (hidden when not checked) */
    .checkmark:after {
        content: "";
        position: absolute;
        display: none;
    }

    /* Show the checkmark when checked */
    .container2 input:checked ~ .checkmark:after {
        display: block;
    }

    /* Style the checkmark/indicator */
    .container2 .checkmark:after {
        left: 4px;
        top: 0px;
            width: 7px;
    height: 12px;
        border: solid white;
        border-width: 0 3px 3px 0;
        -webkit-transform: rotate(45deg);
        -ms-transform: rotate(45deg);
        transform: rotate(45deg);
    }
</style>
