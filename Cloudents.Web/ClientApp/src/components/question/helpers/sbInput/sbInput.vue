<template>
    <div  :class="['input-wrapper', prependInnerIcon ? 'input-wrapper-bordered': '']">
        <span v-if="!bottomError" class="error-message">{{errorTextStr}}</span>
        <v-icon v-if="prependInnerIcon" class="prepend-icon">{{prependInnerIcon}}</v-icon>
        <input :required="required" :class="['input-field', prependInnerIcon ? 'inputPrepIcon': '', {errorTextStr :'invalid'}]"
               :name="name" :type="type" :bottomError="bottomError" :disabled="disabled" :readonly="isReadOnly"
               :placeholder="placeholder"  :value="value" :hint="hint"
               @change="hideError()"
               @input="updateValue($event.target.value)" :autofocus="autofocus" v-language:placeholder>
        <v-icon v-if="icon">{{icon}}</v-icon>
        <span v-if="hint"  class="input-hint">{{hint}}</span>
        <span v-if="bottomError" class="error-message relative">{{errorTextStr}}</span>
    </div>

    <!--TODO refactor to use v-text field with validation rules-->
    <!--<v-text-field :value="value"-->
    <!--:label="label"-->
    <!--outline-->
    <!--:name="name"-->
    <!--:class="['input-field', prependInnerIcon ? 'inputPrepIcon': '', {errorMessage :'invalid'}]"-->
    <!--:type="type"-->
    <!--:readonly="isReadOnly"-->
    <!--:rules="rules"-->
    <!--@input="updateValue($event.target.value)" v-language:placeholder></v-text-field>-->


</template>
<script src="./sbInput.js"></script>

<style src="./sbInput.less" lang="less"></style>