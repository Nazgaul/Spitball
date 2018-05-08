<template>
    <div>
        <step-template>
            <div slot="step-data">
                <h1>Your Username</h1>
                <p>We gave you a username. Keep it, or create your own, this is how you’ll be known on the
                    marketplace.</p>
                <v-text-field v-model="username"></v-text-field>

                <button class="continue-btn" @click="next">Continue</button>
            </div>
            <img slot="step-image" :src="require(`../../img/username.png`)"/>
        </step-template>
    </div>
</template>
<script src="./registerUsername.js"></script>
<style src="./registerUsername.less" lang="less"></style>
