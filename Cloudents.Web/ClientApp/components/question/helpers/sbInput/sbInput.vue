<template>
    <div  :class="['input-wrapper', prependInnerIcon ? 'input-wrapper-bordered': '']">
        <span v-if="!bottomError" class="error-message">{{errorMessage}}</span>
        <v-icon v-if="prependInnerIcon" class="prepend-icon">{{prependInnerIcon}}</v-icon>
        <input :required="required" :class="['input-field', prependInnerIcon ? 'inputPrepIcon': '', {errorMessage :'invalid'}]"
               :name="name" :type="type" :bottomError="bottomError" :disabled="disabled" :readonly="isReadOnly"
               :placeholder="placeholder"  :value="value" :hint="hint"
               @input="updateValue($event.target.value)" :autofocus="autofocus" v-language:placeholder>
        <v-icon v-if="icon">{{icon}}</v-icon>
        <span v-if="hint"  class="input-hint">{{hint}}</span>
        <span v-if="bottomError" class="error-message relative">{{errorMessage}}</span>

    </div>
</template>
<script src="./sbInput.js"></script>

<style src="./sbInput.less" lang="less"></style>