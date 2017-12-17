
<template v-once>
    <page-layout :links="links" :submitFunction="search" :items="items" :sites="sites" :selectosFunction="selectos"
                 :bottomIcons="bottomIcons" :testimonials="testimonials">
        <logo slot="logo" class="logo"></logo>
        <v-text-field slot="inputField" solo @keyup.enter="search" autocomplete="off" type="text" required name="q" :class="{'record':isRecording}" id="transcript" v-model.trim="msg" :placeholder="placeholder" prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>
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
<style src="./home.less" lang="less"></style>
<script src="./home.js"></script>