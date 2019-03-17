<template>
    <ul style="padding-right:15px;padding-left: 5px;">
        <li @click="injectToTextArea(`\\times`)">times</li>
        <li @click="injectToTextArea(`\\div`)">div</li>
        <li @click="injectToTextArea(`\\pm`)">pm</li>
        <li @click="injectToTextArea(`\\mp`)">mp</li>
        <li @click="injectToTextArea(`\\cdot`)">cdot</li>
        <li @click="injectToTextArea(`\\star`)">star</li>
        <li @click="injectToTextArea(`\\sqrt{ab}`)">sqrt</li>
        <li @click="injectToTextArea(`\\sqrt[n]{ab}`)">sqrt[n]{ab}</li>
        <li @click="injectToTextArea(`\\log_{a}{b}`)">log_{a}{b}</li>
    </ul>
</template>

<script>
export default {
    props:{
        injectToTextArea:{
            type:Function
        }
    }
}
</script>

<style>

</style>
