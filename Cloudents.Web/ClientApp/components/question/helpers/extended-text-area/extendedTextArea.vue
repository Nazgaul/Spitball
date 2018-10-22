<template>
    <v-flex xs12 class="question_area" :class="{'has-error': error && error.errorClass &&  value.length < 15}">
        <div class="textarea">
            <div class="text-block" :class="`sbf-card-${activeColor.name}`">
                <span class="error-message" v-if="error.errorClass &&  value.length < 15" :error="errorTextArea">{{error.errorText}}</span>
                <textarea rows="9"  required
                          :class="[`sbf-font-${activeColor.name}`, { active: activeColor.id !== 0 }]"
                          @input="updateValue($event.target.value)"
                          :value="value" autofocus="isFocused"
                          :placeholder="`extendedTextArea_type_your_${actionType}`" v-language:placeholder></textarea>
                <div class="action-holder">
                    <ul class="actions_text files-actions">
                        <li>
                            <label for="file-input" class="attach-file" v-show="previewList.length < uploadLimit">
                                <v-icon style="cursor: pointer;" :class="`sbf-font-${activeColor.name}`">sbf-attach</v-icon>
                            </label>
                            <input id="file-input" type="file" multiple accept="image/*"/>
                        </li>
                    </ul>
                    <v-divider v-if="actionType ==='question'" vertical></v-divider>
                    <ul class="actions_text colors-actions" v-if="actionType ==='question'">
                        <li v-for="color in colorsSet" :key="color.id">
                            <button :class="[`sbf-card-${color.name}`, { active: color.id === activeColor.id ||  color.id === 0 }]"
                                    @click="updateColor(color)"></button>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
        <transition name="slide-fade">
            <div class="files pt-3 pb-2" v-if="previewList.length">
                <ul class="preview-list" v-if="previewList.length">
                    <li v-if="previewList.length" v-for="(image,index) in previewList" :key="index">
                        <button class="hover-trash" @click="deletePreview(index)">
                            <v-icon>sbf-close</v-icon>
                        </button>
                        <img :src="image"/>
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
