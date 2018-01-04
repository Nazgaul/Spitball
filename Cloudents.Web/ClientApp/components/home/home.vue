
<template v-once>
    <page-layout :sites="sites" 
                 :bottomIcons="bottomIcons" :testimonials="testimonials">
        <sb-search slot="search"></sb-search>
        <strips-section slot="stripSection" :strips="strips">
            <component :is="props.type" :class="props.classIcon" slot="stripImage" slot-scope="props"></component>
            <component :is="props.type" slot="stripIcon" slot-scope="props"></component>
        </strips-section>
        <features-section slot="featuresSection" :features="features">
            <component class="item" slot="item" :is="props.type" slot-scope="props"></component>
        </features-section>
        <component :is="`${props.iconName}-icon`" class="social-icon" slot="bottomIcon" slot-scope="props"></component>
    </page-layout>
</template>
<style src="./home.less" lang="less"></style>
<script src="./home.js"></script>