<template>
    <v-flex xs12 class="question_area" :class="{'has-error': error && error.errorClass}">
        <div class="textarea">
            <div class="text-block" :class="`sbf-card-${activeColor.name}`">
                <span class="error-message" v-if="error.errorClass" :error="errorTextArea">{{error.errorText}}</span>
                <textarea rows="9" required
                          @click.prevent="isNewBaller()"
                          :class="[`sbf-font-${activeColor.name}`, { active: activeColor.id !== 0 }, isFirefox ? 'firefox-text-area' : '']"
                          @input="updateValue($event.target.value)"
                          :value="value" autofocus="isFocused"
                          :placeholder="`extendedTextArea_type_your_${actionType}`" v-language:placeholder></textarea>
                <div class="action-holder">
                    <ul class="actions_text files-actions" v-if="isAttachVisible">
                        <li>
                            <v-icon style="cursor: pointer; font-size: 18px; position: absolute; top: 4px;"
                                    :class="`sbf-font-${activeColor.name}`">sbf-attach
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
                            <!--<input id="file-input" type="file" multiple accept="image/*"/>-->
                        </li>
                    </ul>
                    <!--<v-divider v-if="actionType ==='question'" vertical></v-divider>-->
                    <ul class="actions_text colors-actions" v-if="actionType ==='question' && false">
                        <li v-for="color in colorsSet" :key="color.id">
                            <button :class="[`sbf-card-${color.name}`, { active: color.id === activeColor.id ||  color.id === 0 }]"
                                    @click="updateColor(color)"></button>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
        <transition name="slide-fade">
            <div class="files pt-3 pb-2" v-if="files.length" >
                <ul class="preview-list" v-if="files.length">
                    <li v-if="files.length" v-for="(file, index) in files" :key="index">
                        <button class="hover-trash" @click="deletePreview(file, index)">
                            <v-icon>sbf-close</v-icon>
                        </button>
                        <img :src="file.blob"/>
                    </li>
                    <!--<li class="add-file" v-show="previewList.length < uploadLimit">-->
                    <!--<label for="file-input">-->
                    <!--<v-icon>sbf-close</v-icon>-->
                    <!--</label>-->
                    <!--</li>-->
                </ul>
            </div>
        </transition>
    </v-flex>
</template>
<script src="./extendedTextArea.js"></script>
<style src="./extendedTextArea.less" lang="less"></style>
