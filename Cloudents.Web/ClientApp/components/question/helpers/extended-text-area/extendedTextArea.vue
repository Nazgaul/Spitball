<template>
        <v-flex xs12 class="question_area"  :class="{'has-error': error && error.errorClass &&  value.length < 15}">
            <div class="textarea">
                <!-- :class="{'with-preview':previewList.length}" v-if="!fullPreview" -->
                <div class="text-block" >
                    <span class="error-message" v-if="error.errorClass &&  value.length < 15"  :error ="errorTextArea" >{{error.errorText}}</span>

                    <textarea rows="9" class="" @input="updateValue($event.target.value)"
                        :value="value" autofocus="isFocused"
                        :placeholder="`Type your ${actionType}...`"></textarea>
                    <ul class="actions_text">
                        <!--<li>-->
                            <!--<button class="insert-equation">-->
                                <!--<v-icon>sbf-pi-symbol</v-icon>-->
                            <!--</button>-->
                        <!--</li>-->
                        <!--<li>-->
                            <!--<button class="insert-symbol">-->
                                <!--<v-icon>sbf-omega-symbol</v-icon>-->
                            <!--</button>-->
                        <!--</li>-->
                        <li>
                            <label for="file-input" class="attach-file">
                                <v-icon>sbf-attach</v-icon>
                            </label>
                            <input id="file-input" type="file" multiple accept='image/*'/>
                        </li>
                    </ul>
                    <!--<ul class="actions_text">-->
                        <!--<li>-->
                            <!--<button class="icon"></button>-->
                        <!--</li>-->
                        <!--<li>-->
                            <!--<button class="icon icon-red"></button>-->
                        <!--</li>-->
                        <!--<li>-->
                            <!--<button class="icon icon-purple"></button>-->
                        <!--</li>-->
                    <!--</ul>-->
                </div>            
                
                <!-- <span class="close-btn" v-if="fullPreview" @click="togglePreview">
                    <v-icon right>sbf-close</v-icon>
                </span> -->
            </div>
            <transition name="slide-fade">
            <div class="files pt-3 pb-2" v-if="previewList.length" >
                <ul class="preview-list" v-if="previewList.length">
                    <li v-if="previewList.length" v-for="(image,index) in previewList">
                        <button class="hover-trash" @click="deletePreview(index)">
                            <v-icon>sbf-close</v-icon>
                        </button>
                        <img :src="image" />
                    </li>
                    <li class="add-file">
                        <label for="file-input">
                                <v-icon>sbf-close</v-icon>
                        </label>
                    </li>
                </ul>
            </div>
            </transition>
        </v-flex>
</template>
<script src="./extendedTextArea.js"></script>
<style src="./extendedTextArea.less" lang="less"></style>
