#!/bin/bash
sharpen_path="/home/user/Projects/Sharpen/sharpen_imazen/src/target/sharpencore-0.0.1-SNAPSHOT-jar-with-dependencies.jar"
xmp_path="/home/user/Projects/Sharpen/xmp-core"
xmp_jar="/home/user/Projects/Sharpen/metadata-extractor/Libraries/xmpcore-5.1.2.jar"
me_path="/home/user/Projects/Sharpen/metadata-extractor"
me_jar="/home/user/Projects/Sharpen/metadata-extractor/Output/maven/metadata-extractor-2.8.1-SNAPSHOT.jar"
nunit_jar="/home/user/Projects/Sharpen/metadata-extractor/Libraries/junit-4.11.jar"
net_me_path="/home/user/Projects/Sharpen/n-metadata-extractor"

red='\e[0;31m';
green='\e[0;32m'
NC='\e[0m';

function convert_source(){

  source=$1
  config=$2
  additional_args=$5
  
  java -jar $sharpen_path $source @$config $additional_args


  if ! [ $? -eq 0 ] ; then
    return 1
  fi

  net_destination=$4

  if [ -d $net_destination ]; then
    rm -rf $net_destination
    echo -e "${yellow}Sources deleted: $net_destination${NC}"
  fi

  mv -f $3 $net_destination
  echo -e "${yellow}New sources moved: $net_destination${NC}"
}

function handle_error(){

  code=$1
  step=$2
  
  if [ $code -eq 1 ] ; then
    echo -e "${red}$step conversion error${NC}"
    exit 1
  fi

  if [ $code -eq 2 ] ; then
    echo -e "${red}$step moving error${NC}"
    exit 1
  fi
}

if ! [ -f $me_jar ] || [ -z "$me_jar" ]; then
    echo -e "${yellow}Please build metadata extractor before conversion: ${red}mvn install${NC}"
    exit 1
  fi

# XMP core
converted_source=$xmp_path/`basename $xmp_path`.net/com
convert_source $xmp_path/src $xmp_path/sharpen-all-options-without-configuration $converted_source $net_me_path/Com.Adobe.Xmp/Com
handle_error $? "Xmp core"

# ME 
converted_source=$me_path/`basename $me_path`.net/com
convert_source $me_path/Source $net_me_path/sharpen-all-options $converted_source $net_me_path/Com.Drew/Com "-cp $xmp_jar"
handle_error $? "Metadata extractor"

# ME Tests
converted_source=$me_path/`basename $me_path`.net/com
convert_source $me_path/Tests $net_me_path/sharpen-all-options $converted_source $net_me_path/Com.Drew.Tests/Com "-cp $xmp_jar -cp $me_jar -cp $nunit_jar"
handle_error $? "Metadata extractor Tests"