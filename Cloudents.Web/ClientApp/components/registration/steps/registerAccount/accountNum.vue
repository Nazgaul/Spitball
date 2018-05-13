<template>
    <div>
        <input type="text"  ref="grid" id="input-url" :value="accountNum" readonly>
        <button class="small-button" @click="copyToClipboard">Copy number</button>
    </div>
</template>
<script src="./accountNum.js"></script>
