﻿
<template functional>
    <section class="strips-section">
        <div class="strip" v-for="(strip,index) in props.strips" :class="strip.class" :key="index" >
            <v-layout row justify-center align-center wrap class="strip-content px-2">
                <v-flex class="image-wrapper mb-3" sm12 md6>
                    <div class="images">
                        <lazy-component class="text-xs-center">
                            <img :src="require(`./img/${strip.image}`)" />
                        </lazy-component>
                        <div class="floating-icons">
                            <template v-for="(image,index2) in strip.floatingImages">
                                <slot name="stripImage" :type="image" :class-icon="`icon-${index2+1}`" class="white"></slot>
                            </template>
                        </div>
                    </div>
                </v-flex>
                <v-flex class="description" sm12 md6>
                    <v-layout class="strip-title text-xs-center" row align-center>
                        <slot name="stripIcon" :type="strip.titleIcon"></slot>
                        <h3 class="title-text">{{strip.title}}</h3>
                    </v-layout>
                    <p class="strip-text mt-2 mb-0 text-xs-center text-sm-left">{{strip.text}}</p>
                </v-flex>
            </v-layout>
        </div>
    </section>
</template>



<style src="./horizontalStrip.less" lang="less"></style>