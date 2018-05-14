<template>
    <div>
        <div class="input-wrapper">
            <input type="text" ref="grid" id="input-url" :value="accountNum" readonly>
            <v-icon>sbf-key</v-icon>
        </div>
        <button class="small-button" @click="copyToClipboard"><v-icon>sbf-copy</v-icon> Copy number</button>
    </div>
</template>
<script src="./accountNum.js"></script>
