<template>
    <div>
        <step-template>
            <div slot="step-data" class="limited-width">
                <h1 class="step-title">Your Username</h1>
                <p class="sub-title">We gave you a username. Keep it, or create your own, this is how you’ll be known On
                    the community</p>
                <input class="username-field" disabled v-model="username"/>
                <button class="small-button" @click="editUsername">Edit</button>
                <button class="continue-btn" @click="next">Continue</button>
            </div>
            <img slot="step-image" :src="require(`../../img/username.png`)"/>
        </step-template>
    </div>
</template>
<script src="./registerUsername.js"></script>
<style src="./registerUsername.less" lang="less"></style>
