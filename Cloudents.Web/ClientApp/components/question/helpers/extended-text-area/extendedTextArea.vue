<template>
    <v-flex xs12 class="question_area" :class="{'has-error': error && error.errorClass}">
        <div class="textarea">
            <div class="text-block">
                <span class="error-message" v-if="error.errorClass" :error="errorTextArea">{{error.errorText}}</span>
                <textarea 
                    rows="9" 
                    required
                    @click.prevent="isNewBaller()"
                    @input="updateValue($event.target.value)"
                    :value="value" autofocus="isFocused"
                    :placeholder="setPlaceholder" 
                    v-language:placeholder>
                </textarea>
                <div class="action-holder">
                    <ul class="actions_text files-actions" v-if="isAttachVisible">
                        <li>
                            <v-icon style="cursor: pointer; font-size: 18px; position: absolute; top: 4px;">sbf-attach
                            </v-icon>
                            <file-upload
                                    id="file-input"
                                    :input-id="componentUniqueId"
                                    ref="upload"
                                    :drop="false"
                                    v-model="files"
                                    :multiple="true"
                                    :maximum="4"
                                    :post-action=uploadUrl
                                     accept="image/*"
                                    :extensions="['jpeg', 'jpe', 'jpg', 'gif', 'png', 'webp']"
                                    @input-file="inputFile"
                                    @input-filter="inputFilter">
                            </file-upload>
                        </li>
                    </ul>
                    <span class="error-file-span" v-if="uploadFileError" v-language:inner="'chat_file_error'"></span>
                </div>
            </div>
        </div>
        <transition name="slide-fade">
            <div class="files pt-3 pb-2" v-if="files.length" >
                <ul class="preview-list">
                    <li v-for="(file, index) in files" :key="index">
                        <button class="hover-trash" @click="deletePreview(file, index)">
                            <v-icon>sbf-close</v-icon>
                        </button>
                        <img :src="file.blob"/>
                    </li>
                </ul>
            </div>
        </transition>
    </v-flex>
</template>
<script src="./extendedTextArea.js"></script>
<style src="./extendedTextArea.less" lang="less"></style>
