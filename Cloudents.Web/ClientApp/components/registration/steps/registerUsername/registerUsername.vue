<template>
    <div class="step-username">
        <step-template>
            <div slot="step-data" class="limited-width">
                <h1 class="step-title">Your Username</h1>
                <p class="sub-title">We gave you a username. Keep it, or create your own, this is how you’ll be known On
                    the community</p>

                <!--<sb-input class="phone-field" icon="sbf-phone" name="email" type="tel"></sb-input>-->
                <sb-input icon="sbf-users" v-model="username" :errorMessage="errorMessage" class="username-field input-field" type="text" :editable="editable" :focus="focus"></sb-input>
                <!--<div class="input-wrapper">-->
                    <!--<input class="username-field input-field" disabled v-model="username"/>-->
                    <!--<v-icon>sbf-users</v-icon>-->
                <!--</div>-->
                <button class="small-button" @click="focus=true">Edit</button>
                <button class="continue-btn" @click.once="next" :disabled="submitted">Continue</button>
            </div>
            <img slot="step-image" :src="require(`../../img/username.png`)"/>
        </step-template>
    </div>
</template>
<script src="./registerUsername.js"></script>
<style src="./registerUsername.less" lang="less"></style>
