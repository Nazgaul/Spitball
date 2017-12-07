﻿
<template functional>
    <component is="slot">
        <v-container v-for="(strip,index) in props.strips" class="strip" :class="strip.class" :key="index">
            <v-layout row justify-center align-center class="strip-content limited-width">
                <v-flex class="image-wrapper">
                    <div class="images">
                        <lazy-component>
                            <img :src="require(`./img/${strip.image}`)" />
                        </lazy-component>                        <div class="floating-icons">
                            <template v-for="(image,index2) in strip.floatingImages">
                                <slot name="stripImage" :type="image" :class-icon="`icon-${index2+1}`" class="white"></slot>
                            </template>
                        </div>
                    </div>
                </v-flex>
                <v-flex class="description">
                    <v-layout class="strip-title" row align-center justify-start>
                        <slot name="stripIcon" :type="strip.titleIcon"></slot>
                        <h3 :style="{'color':strip.titleColor}">{{strip.title}}</h3>
                    </v-layout>
                    <p class="strip-text">{{strip.text}}</p>

                </v-flex>
            </v-layout>
        </v-container>
    </component>
</template>



<style src="./horizontalStrip.less" lang="less"></style>