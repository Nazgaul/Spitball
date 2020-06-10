<template>
    <div  :class="['input-wrapper', prependInnerIcon ? 'input-wrapper-bordered': '']">
        <span v-if="!bottomError" class="error-message">{{errorTextStr}}</span>
        <v-icon v-if="prependInnerIcon" class="prepend-icon">{{prependInnerIcon}}</v-icon>
        <input :required="required" :class="['input-field', prependInnerIcon ? 'inputPrepIcon': '', {errorTextStr :'invalid'}]"
               :name="name" :type="type" :bottomError="bottomError" :disabled="disabled" :readonly="isReadOnly"
               :placeholder="$t(placeholder)"  :value="value" :hint="hint"
               @change="hideError()"
               @input="updateValue($event.target.value)" :autofocus="autofocus">
        <v-icon v-if="icon">{{icon}}</v-icon>
        <span v-if="hint"  class="input-hint">{{hint}}</span>
        <span v-if="bottomError" class="error-message relative">{{errorTextStr}}</span>
    </div>
</template>
<script src="./sbInput.js"></script>

<style src="./sbInput.less" lang="less"></style>