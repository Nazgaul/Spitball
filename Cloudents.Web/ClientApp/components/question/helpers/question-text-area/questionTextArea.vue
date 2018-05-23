<template>
    <v-flex xs12 class="question_area">
        <div class="textarea">
            <textarea rows="9" @input="updateValue($event.target.value)" :value="value" autofocus="isFocused"
                        placeholder="Type in your question (Ex: Find the derivative of y = (x + 2)2 - 5x3)"></textarea>
            <ul class="actions_text">
                <li>
                    <button class="insert-equation">
                        <v-icon>sbf-pi-symbol</v-icon>
                    </button>
                </li>
                <li>
                    <button class="insert-symbol">
                        <v-icon>sbf-omega-symbol</v-icon>
                    </button>
                </li>
                <li>
                    <label for="file-input" class="attach-file">
                        <v-icon>sbf-attach</v-icon>
                    </label>
                    <input id="file-input" type="file" multiple/>
                </li>
            </ul>
            <ul class="actions_text">
                <li>
                    <button class="icon"></button>
                </li>
                <li>
                    <button class="icon icon-red"></button>
                </li>
                <li>
                    <button class="icon icon-purple"></button>
                </li>
            </ul>
            <div id="preview-list">
                <span v-for="image in previewList" v-html="image"></span>
            </div>
        </div>
        
    </v-flex>
</template>
<script src="./questionTextArea.js"></script>
<style src="./questionTextArea.less" lang="less"></style>
