<template>
        <v-flex xs12 class="question_area">
            <div class="textarea">
                <div class="text-block" :class="{'with-preview':previewList.length}" v-if="!fullPreview">
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
                </div>            
                <ul class="preview-list" :class="{'full-preview':fullPreview}" v-if="previewList.length">
                    <div class="show-more" v-if="previewList.length>1 && !fullPreview" @click="togglePreview">+ {{previewList.length-1}} more</div> 
                    <!-- v-html="image"  -->
                    <li v-if="previewList.length" v-for="(image,index) in previewList" :class="{'use-hover':previewList.length==1 || fullPreview}">
                        <button class="hover-trash" @click="deletePreview(index)">
                            <v-icon>sbf-trash</v-icon>
                        </button>
                        <div class="background"></div>
                        <img :src="image" />
                    </li>
                </ul>
                <span class="close-btn" v-if="fullPreview" @click="togglePreview">
                    <v-icon right>sbf-close</v-icon>
                </span>
            </div>
        </v-flex>
</template>
<script src="./extendedTextArea.js"></script>
<style src="./extendedTextArea.less" lang="less"></style>
