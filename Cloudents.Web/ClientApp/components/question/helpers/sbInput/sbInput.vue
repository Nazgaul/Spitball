<template>
    <div class="input-wrapper">
        <span class="error-message">{{errorMessage}}</span>
        <input required class="input-field" :name="name" :type="type" :readonly="isReadOnly"
               :placeholder="placeholder" :class="{'invalid': errorMessage}" :value="value"
               @input="updateValue($event.target.value)" :autofocus="autofocus">
        <v-icon v-if="icon">{{icon}}</v-icon>
    </div>
</template>

<script src="./sbInput.js"></script>

<style src="./sbInput.less" lang="less"></style>