<?php

function register_styles(){
    wp_register_style('style', get_template_directory_uri() . '/style.css');
    wp_enqueue_style('style');
}

function load_my_scripts() {
    wp_register_script(
        'my-script', 
        get_template_directory_uri() . '/js.js', 
        array(), 
        '1.0.0', 
        true
    );
    
    wp_enqueue_script('my-script');
    
    wp_localize_script('my-script', 'scriptData', array(
        'templateUrl' => get_template_directory_uri()
    ));
}

add_action('wp_enqueue_scripts', 'register_styles');
add_action('wp_enqueue_scripts', 'load_my_scripts');

register_nav_menu( 'menu', 'Main menu' );