<template>
<page-layout :links="links" :submitFunction="search" :items="items" :sites="sites" :selectosFunction="selectos"
             :bottomIcons="bottomIcons" :voice="{flag:voiceEnable,callback:$_voiceDetection}" :testimonials="testimonials">
    <logo slot="logo" class="logo"></logo>
    <search-icon slot="searchIcon"></search-icon>
    <input slot="inputField" @keyup.enter="search" autocomplete="off" type="text" required name="q" id="transcript" v-model.trim="msg" :placeholder="placeholder">
    <component v-if="voiceEnable" slot="voiceIcon" :is="(isRecording?'recording-icon':'mic-icon')"></component>
    <strips-section slot="stripSection" :strips="strips">
        <component :is="props.type" :class="props.classIcon" slot="stripImage" slot-scope="props"></component>
        <component :is="props.type" slot="stripIcon" slot-scope="props"></component>
    </strips-section>
    <features-section slot="featuresSection" :features="features">
        <component class="item" slot="item" :is="props.type" slot-scope="props"></component>
    </features-section>
    <component :is="`${props.iconName}-icon`" slot="bottomIcon" slot-scope="props"></component>
</page-layout>
</template>

<style src="./home.less" lang="less" scoped></style>
<script src="./home.js"></script>